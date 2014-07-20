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

        public List<StringKeyGroup<db.FoodMatches>> FoodChoices { 
            get 
            { 
                return CreateGroups();
            }  
        }

        public ChooseFood()
        {
            InitializeComponent();
            DataContext = this;
        }

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

        public static List<StringKeyGroup<db.FoodMatches>> CreateGroups()
        {
            // Create List to hold final list
            List<StringKeyGroup<db.FoodMatches>> SortedBatch = new List<StringKeyGroup<db.FoodMatches>>();
            StringKeyGroup<db.FoodMatches> produceFoods = new StringKeyGroup<db.FoodMatches>("produce");
            StringKeyGroup<db.FoodMatches> proteinsFoods = new StringKeyGroup<db.FoodMatches>("proteins");
            StringKeyGroup<db.FoodMatches> dairyFoods = new StringKeyGroup<db.FoodMatches>("dairy");
            StringKeyGroup<db.FoodMatches> grainsFoods = new StringKeyGroup<db.FoodMatches>("grains");
            StringKeyGroup<db.FoodMatches> condimentsFoods = new StringKeyGroup<db.FoodMatches>("condiments");
            StringKeyGroup<db.FoodMatches> miscFoods = new StringKeyGroup<db.FoodMatches>("misc");
            if (matches == null) return SortedBatch;
            // Fill each list with the appropriate FridgeFoods
            foreach (db.FoodMatches match in matches)
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
            return SortedBatch;
        }

        /// <summary>
        /// Called when an item is selected from the list of food matches.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoodSelected(object sender, SelectionChangedEventArgs e)
        {
            db.FoodMatches selectedFood = ((LongListSelector)sender).SelectedItem as db.FoodMatches;
            db.AddFridgeFood(selectedFood);

            // Whoever navigated to this page should have set referrer,
            // so we can set a status back to that page
            string stat = "Food " + selectedFood.FoodName + " entered into fridge!";
            if (referrer == "EnterFoodsManually")
                EnterFoodsManually.status = stat;
            else if (referrer == "EnterFoods")
                EnterFoods.status = stat;
            
            // selection succeeded, go back
            NavigationService.GoBack();
        }
    }
}