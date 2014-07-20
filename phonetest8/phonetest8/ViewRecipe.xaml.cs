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
    public partial class ViewRecipe : PhoneApplicationPage
    {
        public ViewRecipe()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.recipeViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.recipeViewModel.IsDataLoaded)
            {
                App.recipeViewModel.LoadData();
            }
        }

        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        {
            PivotPage.Title = RecipeViewModel.recipe.Name;
        }
    }
}