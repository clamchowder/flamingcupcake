using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace phonetest8
{
    public partial class ChooseFood : PhoneApplicationPage
    {
        public static List<db.FoodMatches> matches;
        public static string referrer = "";

        public List<db.FoodMatches> FoodChoices { 
            get 
            { 
                return matches;
            }  
        }

        public ChooseFood()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Called when an item is selected from the list of food matches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ingListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            db.FoodMatches selectedFood = ((ListBox)sender).SelectedItem as db.FoodMatches;
            db.AddFridgeFood(selectedFood);

            // Whoever navigated to this page should have set referrer,
            // so we can set a status back to that page
            string stat = "\"" + selectedFood.FoodName + "\" placed into my fridge!";
            if (referrer == "EnterFoodsManually")
                EnterFoodsManually.status = stat;
            else if (referrer == "EnterFoodsBarcode")
                EnterFoodsBarcode.status = stat;

            // selection succeeded, go back
            NavigationService.GoBack();
        }
    }
}