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

namespace phonetest8
{
    public partial class Page1 : PhoneApplicationPage
    {
        private PhotoCamera _phoneCamera;
        private IBarcodeReader _barcodeReader;
        private DispatcherTimer _scanTimer;
        private WriteableBitmap _previewBuffer;

        private bool initialize_called;
        private string last_upc;

        public Page1()
        {
            initialize_called = false;
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Initialize the camera object
            _phoneCamera = new PhotoCamera();
           // _phoneCamera.FlashMode = FlashMode.Off;
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
        void initialize_cam(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (initialize_called) return;
            if (e.Succeeded)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Info.Text = "Camera initialized.";
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

                    int desc_offset = 0;
                    int desc_end_offset = 0;
                    responseText = sr.ReadToEnd();
                    desc_offset = responseText.IndexOf("Description") + 29;
                    if (desc_offset == 28) // indexOf returns -1 => 29 - 1 = 28
                    {
                        output += "Could not find product";
                    }
                    else
                    {
                        string desc = responseText.Substring(desc_offset);
                        desc_end_offset = desc.IndexOf("</td>");
                        if (desc_end_offset == -1)
                        {
                            output += "Error parsing return from UPC site";
                        }
                        output += desc.Substring(0, desc_end_offset);
                    }
                    Dispatcher.BeginInvoke(() => /* necessary to prevent access issues */
                    {
                        Info.Text = output;
                    });
                }, null);
            }); 
            
        }

        void scan_for_barcode()
        {
            Dispatcher.BeginInvoke(() =>
            {
                _phoneCamera.GetPreviewBufferArgb32(_previewBuffer.Pixels);
                _previewBuffer.Invalidate();
                _barcodeReader.Decode(_previewBuffer);
            });
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
    }
}