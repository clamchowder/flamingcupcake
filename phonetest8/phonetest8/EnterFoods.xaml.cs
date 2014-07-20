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

using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace phonetest8
{
    public partial class EnterFoods : PhoneApplicationPage
    {
        ProgressIndicator prog;
        private PhotoCamera _phoneCamera = null;
        private IBarcodeReader _barcodeReader;
        private DispatcherTimer _scanTimer;
        private WriteableBitmap _previewBuffer;

        public bool initialize_called;
        private string last_upc;
        private string lastFoodName;

        public static string status = "";

        public static Boolean active = false;

        public EnterFoods()
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

        private async void GetMatches(object sender, RoutedEventArgs e)
        {
            if (lastFoodName == null) return;

            last_upc = "";

            Dispatcher.BeginInvoke(() =>
            {
                Info.Text = "Please wait...";
                MatchesButton.Visibility = Visibility.Collapsed;
                StartInDeterminateProgress("trying to understand what you just scanned");
            });

            // Get matches from azure
            List<db.FoodMatches> matches = await db.getFoodMatches(lastFoodName);

            StopInDeterminateProgress();
            if (matches.Count() == 0)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Info.Text = "Sorry, no matches found! Try again...";
                    MatchesButton.Visibility = Visibility.Collapsed;
                    ManualButton.Visibility = Visibility.Visible;
                });
                return;
            }

            db.FoodMatches bestMatch = null;

            // Find maximum number of keyword matches to rank results
            // also keep track of how many are tied for 'best match'.
            int maxKeywordMatches = 0, numMatchesAtMax = 0;
            foreach (db.FoodMatches match in matches)
            {
                if (match.keywordCount > maxKeywordMatches)
                {
                    maxKeywordMatches = 1;
                    numMatchesAtMax = 1;
                    bestMatch = match;
                }
                else if (match.keywordCount == maxKeywordMatches)
                {
                    numMatchesAtMax++;
                }
            }
            if (numMatchesAtMax > 1)
            {
                // navigate to choose foods page
                ChooseFood.matches = matches;
                ChooseFood.referrer = "EnterFoods";
                NavigationService.Navigate(new Uri("/ChooseFood.xaml", UriKind.Relative));
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Info.Text = bestMatch.FoodName + " is now in the virtual fridge!";
                });
                db.AddFridgeFood(bestMatch);
            }
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            active = true;
            // If status is set, fill in the status text block, and then
            // clear the status
            Info.Text = status;
            status = "";

            // When the user first sees the page, the manual entry
            // option should be avaialble, and the "get matches" button should not
            MatchesButton.Visibility = Visibility.Collapsed;
            ManualButton.Visibility = Visibility.Visible;

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

            // This timer will be used to scan the camera buffer every 100ms and scan for any barcodes
            _scanTimer = new DispatcherTimer();
            _scanTimer.Interval = TimeSpan.FromMilliseconds(100);
            _scanTimer.Tick += (o, arg) => scan_for_barcode();

            viewfinderCanvas.Tap += new EventHandler<GestureEventArgs>(focus_Tapped);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _scanTimer.Stop();
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
                    _scanTimer.Start();
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
                
                if (obj.Text == last_upc) return;
                last_upc = obj.Text;
                Info.Text = output;

                string barcodeValue = obj.Text; 
                WebRequest req = WebRequest.Create("http://www.upcdatabase.com/item/" + barcodeValue);
                req.BeginGetResponse(r =>
                {
                    WebResponse response = req.EndGetResponse(r);
                    Stream resStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
                    string responseText = "";
                    string foodName = "";

                    int desc_offset = 0;
                    int desc_end_offset = 0;
                    responseText = sr.ReadToEnd();
                    desc_offset = responseText.IndexOf("Description") + 29;
                    if (desc_offset == 28) // indexOf returns -1 => 29 - 1 = 28
                    {
                        output += "Could not find product";
                        Dispatcher.BeginInvoke(() => /* necessary to prevent access issues */
                        {
                            Info.Text = output;
                            ManualButton.Visibility = Visibility.Visible;
                        });
                    }
                    else
                    {
                        string desc = responseText.Substring(desc_offset);
                        desc_end_offset = desc.IndexOf("</td>");
                        if (desc_end_offset == -1)
                        {
                            output += "Error parsing return from UPC site";
                        }
                        foodName = desc.Substring(0, desc_end_offset);
                        lastFoodName = foodName;
                        output = foodName;
                    }
                    Dispatcher.BeginInvoke(() => /* necessary to prevent access issues */
                    {
                        Info.Text = output;
                        ManualButton.Visibility = Visibility.Collapsed;
                        MatchesButton.Visibility = Visibility.Visible;
                    });
                }, null);
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
                SystemTray.SetIsVisible(this, false);

                prog.IsVisible = false;
            }
        }
    }
}