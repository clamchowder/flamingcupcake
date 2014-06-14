using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Net;
using System.Text;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Cphonetest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            string barcodeValue = barcode.Text; // TODO: not hardcode this
            WebRequest req = WebRequest.Create("http://www.upcdatabase.com/item/" + barcodeValue);         
            Task<WebResponse> tsk = req.GetResponseAsync();
            WebResponse response = tsk.Result;
            Stream resStream = response.GetResponseStream();

            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
            string responseText = "";

            int desc_offset = 0;
            int desc_end_offset = 0;
            responseText = sr.ReadToEnd();

            string output = "";
            //output += responseText;

            desc_offset = responseText.IndexOf("Description") + 29;
            string desc = responseText.Substring(desc_offset);
            desc_end_offset = desc.IndexOf("</td>");
            output += desc.Substring(0, desc_end_offset);

            outputTextBlock.Text = output;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
