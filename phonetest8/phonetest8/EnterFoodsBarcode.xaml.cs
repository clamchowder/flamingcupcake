using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;  // Needed for PhotoCamera
using System.Windows.Threading; // needed for DispatchTimers
using ZXing; // neeed for IBarcodeReader
using System.Windows.Media.Imaging; // needed for writeableBitmap
using System.Windows.Input; // needed for GestureEventArgs
using System.Threading.Tasks; // used for async web response
using System.IO; // needed for stream (for web response, for UPC code lookup online)

using WindowsPreview.Media.Ocr;

using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace phonetest8
{
    public partial class EnterFoodsBarcode : PhoneApplicationPage
    {
        ProgressIndicator prog;
        private PhotoCamera _phoneCamera = null;
        private IBarcodeReader _barcodeReader;
        private WriteableBitmap _previewBuffer;

        public bool initialize_called;
        private string last_upc;
        private string lastFoodName;

        private db.FoodMatches lastFoodMatch = null;

        public static string status = "";

        public static Boolean active = false;

        public EnterFoodsBarcode()
        {
            initialize_called = false;
            InitializeComponent();
            MatchesButton.Visibility = Visibility.Collapsed;
            lastFoodName = null;
            active = true;
        }

        private void GoManual(object sender, RoutedEventArgs e)
        {
            initialize_called = false;
            NavigationService.Navigate(new Uri("/EnterFoodsManually.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Called when the "Put in fridge" button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMatches(object sender, RoutedEventArgs e)
        {
            if (lastFoodName == null) return;

            last_upc = "";

            Dispatcher.BeginInvoke(() =>
            {
                Info.Text = lastFoodMatch.FoodName + " is now in the virtual fridge!";
            });
            db.AddFridgeFood(lastFoodMatch);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            active = true;
            // If status is set, fill in the status text block, and then
            // clear the status
            Info.Text = status;
            status = "";

            // Start with the "Get Matches" button invisible
            // TODO: Put in a "Scan Barcode" button
            MatchesButton.Visibility = Visibility.Collapsed;

            // Initialize the camera object
            _phoneCamera = new PhotoCamera();
            StartInDeterminateProgress("initializing camera");
            _phoneCamera.Initialized += initialize_cam;

            _barcodeReader = new BarcodeReader();
            _barcodeReader.Options.TryHarder = true;
            _barcodeReader.ResultFound += result_found;

            //Display the camera feed in the UI
            viewfinderBrush.SetSource(_phoneCamera);
            _previewBuffer = new WriteableBitmap((int)_phoneCamera.PreviewResolution.Width, (int)_phoneCamera.PreviewResolution.Height);

            viewfinderCanvas.Tap += new EventHandler<GestureEventArgs>(focus_Tapped);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //_phoneCamera.Dispose();
            //_phoneCamera = null;
            initialize_called = false;
            active = false;
            base.OnNavigatedFrom(e);
        }

        void initialize_cam(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (initialize_called) return;
            if (e.Succeeded)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Info.Text = "Camera initialized.";
                    StopInDeterminateProgress();
                });          
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Info.Text = "Could not initialize camera.";
                });
            }
            initialize_called = true;
        }

        void result_found(Result obj)
        {
            Dispatcher.BeginInvoke(() =>
            {
                string output = "Format: " + obj.BarcodeFormat + ", Result: " + obj.Text + "\n";
                string foodName = "";

                if (obj.Text == last_upc) return;
                last_upc = obj.Text;
                Info.Text = output;

                string barcodeValue = obj.Text; 
                // First see if the barcode/associated result is cached (disabled for testing)
                //foodName = db.GetBarcodeResultFromCache(barcodeValue);
                foodName = null;
                if (foodName == null)
                {
                    // Cache miss. Make a web request to get a result
                    WebRequest req = WebRequest.Create("http://foodstormfun.cloudapp.net/upc.php?code=" + barcodeValue);
                    req.BeginGetResponse(r =>
                    {
                        WebResponse response = req.EndGetResponse(r);
                        Stream resStream = response.GetResponseStream();
                        StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
                        string responseText = "";

                        responseText = sr.ReadToEnd();
                        if (responseText.Equals("No result"))
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                Info.Text = "No result :(";
                            });
                        }
                        else
                        {
                            char[] responseSep = new char[1];
                            responseSep[0] = ',';
                            string[] responseArr = responseText.Split(responseSep);
                            
                            if (lastFoodMatch == null) lastFoodMatch = new db.FoodMatches();

                            foodName = responseArr[1];

                            lastFoodMatch.FoodName = responseArr[1];
                            lastFoodMatch.foodId = responseArr[0];
                            lastFoodName = foodName; // populate lastFoodName - this is used by GetMatches
                            db.AddBarcodeResultToCache(barcodeValue, foodName);
                            Dispatcher.BeginInvoke(() =>
                            {
                                // Update UI in a separate thread to prevent access issues
                                Info.Text = "Looks like this is " + lastFoodName;
                                MatchesButton.Visibility = Visibility.Visible;
                            });
                        }
                        
                    }, null);
                }
                else
                {
                    // Name is not null, so it's in the cache.
                    lastFoodName = foodName;
                    output = foodName;
                    Dispatcher.BeginInvoke(() => 
                    {
                        // Update UI
                        Info.Text = output;
                        MatchesButton.Visibility = Visibility.Visible;
                    });
                }
            }); 
            
        }

        void scan_for_barcode()
        {
            if (initialize_called && active)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        _phoneCamera.GetPreviewBufferArgb32(_previewBuffer.Pixels);
                        _previewBuffer.Invalidate();
                        _barcodeReader.Decode(_previewBuffer);
                        
                    }
                    catch (Exception)
                    {
                        initialize_called = false;
                    }
                });
            }
        }

        void focus_Tapped(object sender, GestureEventArgs e)
        {
            if (_phoneCamera != null)
            {
                if (_phoneCamera.IsFocusAtPointSupported == true)
                {
                    // Determine the location of the tap.
                    Point tapLocation = e.GetPosition(viewfinderCanvas);

                    // Position the focus brackets with the estimated offsets.
                    focusBrackets.SetValue(Canvas.LeftProperty, tapLocation.X - 30);
                    focusBrackets.SetValue(Canvas.TopProperty, tapLocation.Y - 28);

                    // Determine the focus point.
                    double focusXPercentage = tapLocation.X / viewfinderCanvas.ActualWidth;
                    double focusYPercentage = tapLocation.Y / viewfinderCanvas.ActualHeight;

                    // Show the focus brackets and focus at point.
                    focusBrackets.Visibility = Visibility.Visible;
                    try
                    {
                        _phoneCamera.FocusAtPoint(focusXPercentage, focusYPercentage);
                    }
                    catch (InvalidOperationException)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            Info.Text = "Cannot initiate AF until previous AF operation has completed";
                        });
                    }
                }
            }
        }

        private void StartInDeterminateProgress(String text)
        {
            if (prog == null)
            {
                prog = new ProgressIndicator();
            }
            SystemTray.SetIsVisible(this, true);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = text;
            SystemTray.SetProgressIndicator(this, prog);
        }
        private void StopInDeterminateProgress()
        {
            if (prog != null)
            {
                SystemTray.SetIsVisible(this, true);

                prog.IsVisible = false;
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            scan_for_barcode();
        }
    }
}