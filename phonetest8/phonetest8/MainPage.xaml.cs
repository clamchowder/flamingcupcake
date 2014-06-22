using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using phonetest8.Resources;
//using SQLite;
using Sqlite;

using Windows.Storage;

namespace phonetest8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
         //BuildLocalizedApplicationBar();
        }

        private void RunQuery(object sender, RoutedEventArgs e)
        {
            //Database db = new Database(ApplicationData.Current.LocalFolder, "test.db");
            /*SQLiteConnection conn = new SQLiteConnection(ApplicationData.Current.LocalFolder + "test.db"); 
            SQLiteCommand cmd = new SQLiteCommand(conn);
            string output = "";
            List<List<string>> QueryResult = null;

            cmd.CommandText = InputTextBox.Text;
            try
            {
                QueryResult = cmd.ExecuteQuery<List<string>>();
            }
            catch (SQLiteException queryException)
            {
                output = queryException.Message;
            }
            if (QueryResult != null)
            {
                output += "Query returned " + QueryResult.Count() + " rows\n";
                int i, j;
                List<string>[] QueryResultArr = QueryResult.ToArray();
                string[] arr = null;
                for (i = 0; i < QueryResultArr.Length; i++)
                {
                    arr = QueryResultArr[i].ToArray();
                    for (j = 0; j < arr.Length; j++)
                    {
                        if (j != 0) output += ", ";
                        output += arr[j];
                    }
                    output += "\n";
                }
            }
            Result.Text = output;*/
        }

        private void RunBarcode(object sender, RoutedEventArgs e)
        {
        }

        private void EnterFoodsClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterFoods.xaml", UriKind.Relative));
        }
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}