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
    public partial class EnterFoodsManually : PhoneApplicationPage
    {
        public static List<db.FoodMatches> rawMatches;
        public static List<StringKeyGroup<db.FoodMatches>> matches;

        // A status to display below the entry button
        // If status is set, text box should be cleared
        public static string status = "";

        /// <summary>
        /// Class with a key to hold food matches
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class StringKeyGroup<T> : List<T>
        {
            /// <summary>
            /// The Key of this group.
            /// </summary>
            public string Key { get; private set; }

            public StringKeyGroup(string key)
            {
                Key = key;
            }
        }

        /// <summary>
        /// Constructor. Clears list of matches
        /// </summary>
        public EnterFoodsManually()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // apply status if set by ChooseFoods (to give confirmation to user)
            // then clear status so it doesn't appear again next time the page is navigated to 
            Status.Text = status;
            status = "";
        }
        /// <summary>
        /// Gets matches using Azure API, then navigates to page that
        /// presents user with a list of choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FindMatches(object sender, RoutedEventArgs e)
        {
            string input = FoodInputBox.Text;
            if (input.Length == 0)
            {
                return;
            }
            rawMatches = await db.getFoodMatches(input);
            if (rawMatches.Count == 0)  // alert user if no match is found
            {
                Dispatcher.BeginInvoke(() => /* necessary to prevent access issues */
                {
                    Status.Text = "No match found!";
                });
                return;
            }
            // Matches found, navigate to page that lets users pick
            ChooseFood.matches = rawMatches;
            ChooseFood.referrer = "EnterFoodsManually";
            NavigationService.Navigate(new Uri("/ChooseFood.xaml", UriKind.Relative));
        }

        public static void CreateGroups()
        {
            // Create List to hold final list
            List<StringKeyGroup<db.FoodMatches>> SortedBatch = new List<StringKeyGroup<db.FoodMatches>>();
            StringKeyGroup<db.FoodMatches> produceFoods = new StringKeyGroup<db.FoodMatches>("produce");
            StringKeyGroup<db.FoodMatches> proteinsFoods = new StringKeyGroup<db.FoodMatches>("proteins");
            StringKeyGroup<db.FoodMatches> dairyFoods = new StringKeyGroup<db.FoodMatches>("dairy");
            StringKeyGroup<db.FoodMatches> grainsFoods = new StringKeyGroup<db.FoodMatches>("grains");
            StringKeyGroup<db.FoodMatches> condimentsFoods = new StringKeyGroup<db.FoodMatches>("condiments");
            StringKeyGroup<db.FoodMatches> miscFoods = new StringKeyGroup<db.FoodMatches>("misc");
            if (rawMatches == null) matches = new List<StringKeyGroup<db.FoodMatches>>();
            // Fill each list with the appropriate FridgeFoods
            foreach (db.FoodMatches match in rawMatches)
            {
                // use instruction as key, find instruction's list
                if (match.foodGroup == "produce") produceFoods.Add(match);
                else if (match.foodGroup == "proteins") proteinsFoods.Add(match);
                else if (match.foodGroup == "diary") dairyFoods.Add(match);
                else if (match.foodGroup == "grains") grainsFoods.Add(match);
                else if (match.foodGroup == "condiments") condimentsFoods.Add(match);
                else miscFoods.Add(match);
            }
            SortedBatch.Add(produceFoods);
            SortedBatch.Add(proteinsFoods);
            SortedBatch.Add(dairyFoods);
            SortedBatch.Add(grainsFoods);
            SortedBatch.Add(condimentsFoods);
            SortedBatch.Add(miscFoods);
            matches = SortedBatch;
        }
    }
}