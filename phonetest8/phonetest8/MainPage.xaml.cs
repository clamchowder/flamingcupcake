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

namespace phonetest8
{
    public partial class MainPage : PhoneApplicationPage
    {
        ProgressIndicator prog;
        public enum FoodGroup
        {
            produce, proteins, dairy, grains, condiments, misc
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Make sure barcode scanning page does not consume excessive CPU cycles
            EnterFoods.active = false;
            base.OnNavigatedFrom(e);
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
            StartInDeterminateProgress("looking for suitable recipes");
            List<db.Recipe> matches = await db.FindRecipes();
            StopInDeterminateProgress();
            MatchedRecipeList.RecipeList = matches;
            NavigationService.Navigate(new Uri("/MatchedRecipeList.xaml", UriKind.Relative));
        }

        private async void GotoFind(object send, RoutedEventArgs e)
        {
            StartInDeterminateProgress("fetching all recipes");
            List<db.Recipe> matches = await db.GetAllRecipes();
            StopInDeterminateProgress();
            MatchedRecipeList.RecipeList = matches;
            NavigationService.Navigate(new Uri("/MatchedRecipeList.xaml", UriKind.Relative));
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
    }
}