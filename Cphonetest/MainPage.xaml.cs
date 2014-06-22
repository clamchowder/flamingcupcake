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

using SQLite;
using Windows.Storage;

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

        private void DBClick(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(ApplicationData.Current.LocalFolder + "test.db");
            SQLiteCommand cmd = new SQLiteCommand(conn);
            string output = "";
            List<string> outputList;
            int rc;
            /*cmd.CommandText = "CREATE DATABASE test";
            rc = cmd.ExecuteNonQuery();
            output += "Create database: " + rc + " rows affected\n"; */

            /*cmd.CommandText = "CREATE TABLE recipe (name TEXT, points INTEGER, required INTEGER)";
            rc = cmd.ExecuteNonQuery();
            output += "Create recipie table: " + rc + " rows affected\n";

            cmd.CommandText = "CREATE TABLE recipeFood (rid, fid)";
            rc = cmd.ExecuteNonQuery();
            output += "Create recipeFood table: " + rc + " rows affected\n";

            cmd.CommandText = "CREATE TABLE food (name TEXT, category TEXT)";
            rc = cmd.ExecuteNonQuery();
            output += "Create food table: " + rc + " rows affected\n";*/

            cmd.CommandText = barcode.Text;
            output = cmd.ExecuteScalar<string>();

            /*cmd.CommandText = "DROP TABLE recipe";
            rc = cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE food";
            rc = cmd.ExecuteNonQuery();
            cmd.CommandText = "DROP TABLE recipeFood";
            rc = cmd.ExecuteNonQuery();*/

            /*string[] outputArr = outputList.ToArray();
            for (int i = 0; i < outputArr.Length; i++)
                output += outputArr[i] + "\n";*/
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
