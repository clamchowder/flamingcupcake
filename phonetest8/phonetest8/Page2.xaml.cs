﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Media.Capture;
using ZXing;
using Windows.Storage;
//using Sqlite;
using SQLite;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace phonetest8
{
    public partial class Page2 : PhoneApplicationPage
    {
        //private SQLiteConnection conn;
        /* why does the example use two combines, like this? */
        public static string db_path;
        private List<db.FoodMatches> matches = null;
        private List<db.Recipe> recipe_matches = null;
        private IMobileServiceTable<db.Food> FoodTable = App.MobileService.GetTable<db.Food>();
        private IMobileServiceTable<db.Recipe> RecipeTable = App.MobileService.GetTable<db.Recipe>();
        private IMobileServiceTable<db.FoodRecipe> FoodRecipeTable = App.MobileService.GetTable<db.FoodRecipe>();
        private IMobileServiceTable<db.Keyword> KeywordTable = App.MobileService.GetTable<db.Keyword>();

        public Page2()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

            // Apparently this creates if the table doesn't already exist
            // Hopefully, it won't overwrite existing tables
           // conn.CreateTable<Food>();
        }
        private async void GetFood(object sender, RoutedEventArgs e)
        {
            //string output = "";
            
            /*List<Food> foods = conn.Table<Food>().ToList<Food>();
            Food[] foods_arr = foods.ToArray();
            for (int i = 0; i < foods_arr.Length; i++)
            {
                output += foods_arr[i] + "\n";
            }*/
            /*await App.MobileService.GetTable<Food>().ToListAsync();
            Result.Text = output;*/
            
            var result = await db.getFoodMatches(InputTextBox.Text);
            matches = result;
            db.FoodMatches[] result_arr = result.ToArray();
            for (int i = 0; i < result_arr.Length; i++)
            {
                Result.Text += result_arr[i].foodName + ", " + result_arr[i].keywordCount + "\n";
            }
        }

        private void AddFood(object sender, RoutedEventArgs e)
        {
            if(matches != null) 
                db.AddFridgeFood(matches.ToArray()[int.Parse(InputTextBox.Text)]);
        }

        private async void GetRecipe(object sender, RoutedEventArgs e)
        {
            var result = await db.FindRecipes();
            db.Recipe[] result_arr = result.ToArray();
            Result.Text = "";
            for (int i = 0; i < result_arr.Length; i++)
            {
                Result.Text += result_arr[i].name + "\n";
            }
            recipe_matches = result;

            List<db.Instruction> instructions = result_arr[0].GetInstructions();
            Result.Text += instructions.Count();
        }
        private async void GetIngredients(object sender, RoutedEventArgs e)
        {
            if (recipe_matches != null)
            {
                var ingredients = await db.getRecipeIngredients(recipe_matches[0]);
                RecipeView.Ingredient[] arr = ingredients.ToArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    Result.Text += arr[i].Name + "\n";
                }
            }
        }
    }
}