using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Recipe = phonetest8.db.Recipe;

namespace phonetest8    
{
    public partial class MatchedRecipeList : PhoneApplicationPage
    {
        private static List<Recipe> _recipeList;

        public static List<Recipe> RecipeList
        {
            get { return _recipeList; }
            set { _recipeList = value; }
        }
        public MatchedRecipeList()
        {
            InitializeComponent();

            matchedRecipelist.DataContext = this;
            try
            {
                matchedRecipelist.ItemsSource = RecipeList;
            }
            catch (Exception)
            {

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Message.Visibility = Visibility.Collapsed;
            if (RecipeList == null || RecipeList.Count == 0)
            {
                if (db.GetFridgeFoods().Count == 0)
                    Message.Text = ":( There's nothing in the fridge. Go grocery shopping!";
                else
                    Message.Text = ":( We couldn't find anything that matched what you have...";
                Message.Visibility = Visibility.Visible;
            }
            base.OnNavigatedFrom(e);
        }

        private void matchedRecipelist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecipeViewModel.recipe = matchedRecipelist.SelectedItem as db.Recipe;
            RecipeViewModel.CreateInstructions(); // TODO: replace with actual passing List<Instructions<Ingredients>> over.
            NavigationService.Navigate(new Uri("/ViewRecipe.xaml", UriKind.Relative));
        }
    }
}