﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using phonetest8.Resources;

namespace phonetest8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public enum FoodGroup
        {
            produce, proteins, dairy, grains, condiments, misc
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void GotoScan(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterFoods.xaml", UriKind.Relative));
        }

        private void GotoFridge(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Fridge.xaml", UriKind.Relative));
        }

        private async void GotoMatch(object send, RoutedEventArgs e)
        {
            List<db.Recipe> matches = await db.FindRecipes();
            MatchedRecipeList.RecipeList = matches;
            NavigationService.Navigate(new Uri("/MatchedRecipeList.xaml", UriKind.Relative));
        }

        private void GotoFind(object send, RoutedEventArgs e)
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