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
    public partial class EnterFoodsOcr : PhoneApplicationPage
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

        private OcrEngine ocrEngine = new OcrEngine(OcrLanguage.English);

        public EnterFoodsOcr()
        {
            initialize_called = false;
            InitializeComponent();
            lastFoodName = null;
            active = true;
        }

        private void GoManual(object sender, RoutedEventArgs e)
        {
            initialize_called = false;
            NavigationService.Navigate(new Uri("/EnterFoodsManually.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            active = true;
            // If status is set, fill in the status text block, and then
            // clear the status
            Info.Text = status;
            status = "";

            // Initialize the camera object
            _phoneCamera = new PhotoCamera();
            StartInDeterminateProgress("initializing camera");
            _phoneCamera.Initialized += initialize_cam;

            //Display the camera feed in the UI
            viewfinderBrush.SetSource(_phoneCamera);
            _previewBuffer = new WriteableBitmap((int)_phoneCamera.PreviewResolution.Width, (int)_phoneCamera.PreviewResolution.Height);

            // Refresh the stored WritableBitmap buffer every 100 ms
            // Although hitting recognize will pull the preview buffer, that's an async operation,
            // so it's a race condition (OCR could be attempted before the bitmap is populated
            _scanTimer = new DispatcherTimer();
            _scanTimer.Interval = TimeSpan.FromMilliseconds(100);
            _scanTimer.Tick += (o, arg) => refresh_preview_buffer();

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

        /**
         * Pulls a preview buffer from the camera feed, and 
         * populates the local copy that's used for OCR.*/
        void refresh_preview_buffer()
        {
            if (initialize_called && active)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        _phoneCamera.GetPreviewBufferArgb32(_previewBuffer.Pixels);
                    }
                    catch (Exception)
                    {
                        initialize_called = false;
                    }
                });
            }
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

        private async void OcrButton_Click(object sender, RoutedEventArgs e)
        {
            refresh_preview_buffer();

            // holds preview buffer rotated 90 degrees cw
            WriteableBitmap rotatedBuffer = new WriteableBitmap(_previewBuffer.PixelHeight, _previewBuffer.PixelWidth);
            // Convert previewBuffer pixels into a byte array
            byte[] inbytes = new byte[_previewBuffer.Pixels.Length * 4];
            byte[] temp = null; // for holding BitConverter output

            // Copy into byte array. Seems like "BGRA8" for ocrEngine and ARGB32 for writeableBitmap have the same byte order
            for (int i = 0; i < _previewBuffer.Pixels.Length; i++)
            {
                temp = BitConverter.GetBytes(_previewBuffer.Pixels[i]);
                inbytes[i * 4] = temp[0];
                inbytes[i * 4 + 1] = temp[1];
                inbytes[i * 4 + 2] = temp[2];
                inbytes[i * 4 + 3] = temp[3];
            }
            OcrResult result = await ocrEngine.RecognizeAsync((uint)_previewBuffer.PixelHeight, (uint)_previewBuffer.PixelWidth, inbytes);
            string text = "";
            if (result.Lines != null)
            {
                foreach (OcrLine line in result.Lines) /* wow what a good way to present info */
                {
                    foreach (OcrWord word in line.Words)
                    {
                        text += word.Text + " ";
                    }
                    text += "\n";
                }
            }
            else text = "Nothing recognized";
            Dispatcher.BeginInvoke(() =>
            {
                //Info.Text = text;
                MessageBox.Show(text);
            });
        }
    }
}