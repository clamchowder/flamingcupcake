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
    public partial class FridgePage : PhoneApplicationPage
    {
        public enum foodGroup
        {
            produce, proteins, dairy, grains, condiments, misc
        }

        // Constructor
        public FridgePage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.fridgeViewModel;
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["SelectAppBar"];

            //BuildLocalizedApplicationBar();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Refresh! Otherwise there's a good chance that it'll keep stale data
            // It's pulling from an on-phone database, so this should be fairly fast
            App.fridgeViewModel.LoadData();

            // If the fridge is empty, show the empty fridge text and disable appbar
            if (App.fridgeViewModel.UnsortedBatch.Count == 0)
            {
                EmptyFridgeText.Visibility = Visibility.Visible;
                FridgeLongListMultiSelector.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
            }
            else
            {
                EmptyFridgeText.Visibility = Visibility.Collapsed;
                FridgeLongListMultiSelector.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
            }
        }

        private void ApplicationBarSelect_Click(object sender, EventArgs e)
        {
            FridgeLongListMultiSelector.IsSelectionEnabled = true;
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DeleteAppBar"];
        }

        private void ApplicationBarDelete_Click(object sender, EventArgs e)
        {
            // Delete from fridge
            List<FridgeFood> delItems = FridgeLongListMultiSelector.SelectedItems as List<FridgeFood>;
            foreach (Object o in FridgeLongListMultiSelector.SelectedItems)
            {
                FridgeFood delItem = o as FridgeFood;
                db.DeleteFridgeFood(delItem.foodId);
            }
            FridgeLongListMultiSelector.IsSelectionEnabled = false;
            ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["SelectAppBar"];

            // Refresh!
            DataContext = new FridgeViewModel();

            if (App.fridgeViewModel.UnsortedBatch.Count == 0)
            {
                EmptyFridgeText.Visibility = Visibility.Visible;
                FridgeLongListMultiSelector.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
            }
            else
            {
                EmptyFridgeText.Visibility = Visibility.Collapsed;
                FridgeLongListMultiSelector.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
            }
        }

        private void ingLongListSelector_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (FridgeLongListMultiSelector.IsSelectionEnabled == false)
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["SelectAppBar"];
            else if (FridgeLongListMultiSelector.IsSelectionEnabled == true)
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["DeleteAppBar"];
            else
                ApplicationBar = (Microsoft.Phone.Shell.ApplicationBar)Resources["SelectAppBar"];
        }

        private void FridgeLongListMultiSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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