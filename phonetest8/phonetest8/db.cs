using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Microsoft.WindowsAzure.MobileServices;
using System.IO;
using System.Windows.Media.Imaging;

namespace phonetest8
{
    public static class db
    {
        private static int max_fridge_id = 0;
        public static MobileServiceClient MobileService = new MobileServiceClient(
            "https://foodstorm.azure-mobile.net/",
            "oGwxtcEwqIfkPUxPiISdOMhyZDihUd78"
        );
        /// <summary>
        /// Gets a list of matching foods, given a real life food name
        /// </summary>
        /// <param name="name">'Raw' name of the food</param>
        /// <returns>await on it for a List of FoodMatches objects</returns>
        public async static Task<List<FoodMatches>> getFoodMatches(string name)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("name", name);
            var result = await MobileService.InvokeApiAsync<List<db.FoodMatches>>("testapi", System.Net.Http.HttpMethod.Get, _params);
            return result;
        }
        /// <summary>
        /// Given a recipe object (with a populated ID field), returns 
        /// </summary>
        /// <param name="recipe"></param>
        /// <returns></returns>
        public async static Task<List<Ingredient>> getRecipeIngredients(Recipe recipe)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("recipeid", recipe.Id);
            var result = await MobileService.InvokeApiAsync<List<Ingredient>>("getingredients", System.Net.Http.HttpMethod.Get, _params);
            return result;
        }
        /// <summary>
        /// Fetches list of foods in fridge, and returns all recipes
        /// that can be made with what's in the fridge
        /// </summary>
        /// <returns></returns>
        public async static Task<List<Recipe>> FindRecipes()
        {
            List<FridgeFood> fridgeFoods = GetFridgeFoods();
            // If the fridge is empty, return an empty list. Don't even go API call.
            if (fridgeFoods.Count == 0)
                return new List<Recipe>();
            Dictionary<string, string> _params = new Dictionary<string, string>();
            FridgeFood[] fridgeFoodsArr = fridgeFoods.ToArray();
            string fridgeStr = "";
            /* throw IDs into a comma separated list
             * Javascript API on the other end takes care of parsing it.
             * This is easier than trying to see how it serialzes a list/arr */
            for (int i = 0; i < fridgeFoodsArr.Length; i++)
            {
                if (i != 0) fridgeStr += ",";
                fridgeStr += fridgeFoodsArr[i].foodId;
            }
            _params.Add("fridge", fridgeStr);
            var result = await MobileService.InvokeApiAsync<List<db.Recipe>>("findrecipe", System.Net.Http.HttpMethod.Get, _params);
            return result;
        }
        /// <summary>
        /// Adds a food to the local fridge database
        /// </summary>
        /// <param name="food">FoodMatch object returned from search (or just populate one yourself with food name and id)</param>
        public static void AddFridgeFood(FoodMatches food)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<FridgeFood>(); // Should do create if not exists, so stuff doesn't get overwritten
            FridgeFood ins = new FridgeFood();
            ins.foodId = food.foodId;
            ins.foodName = food.FoodName;
            ins.foodGroup = food.foodGroup;

            // Do not allow duplicate entries in the fridge
            conn.InsertOrReplace(ins);
        }

        /// <summary>
        /// Directly adds a fridge food object for testing
        /// </summary>
        /// <param name="food"></param>
        public static void AddFridgeFood(FridgeFood food)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<FridgeFood>(); // SHould do create if not exists, so stuff doesn't get overwritten
            FridgeFood ins = new FridgeFood();
            ins.foodId = food.foodId;
            ins.foodName = food.foodName;
            ins.foodGroup = food.foodGroup;
            conn.InsertOrReplace(ins);
        }
        /// <summary>
        /// Deletes the fridge item with the given id (id of fridge object = foodId)
        /// </summary>
        /// <param name="id">ID of fridge object</param>
        public static void DeleteFridgeFood(string id)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.Delete<FridgeFood>(id);
        }

        /// <summary>
        /// Fetches all objects in fridge for local display
        /// </summary>
        /// <returns>A list of Fridge objects</returns>
        public static List<FridgeFood> GetFridgeFoods()
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<FridgeFood>();
            List<FridgeFood> retlist;
            try
            {
                // Attempt to query the Fridge table. This can fail 
                // if the database hasn't been created yet (first time things)
                retlist = conn.Table<FridgeFood>().ToList<FridgeFood>();
            }
            catch (Exception)
            {
                // If the query fails (SQLite throws an exception if the table doesn't exist yet),
                // just return an empty list.
                // want to maintain guarantee that this function will never return null,
                // so that excessive null checks aren't needed elsewhere
                retlist = new List<FridgeFood>();
            }
            return retlist;
        }

        /// <summary>
        /// Given a barcode number in string format, attempts to fetch
        /// the associated name from the cache
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public static string GetBarcodeResultFromCache(string barcode)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<BarcodeResult>();
            string[] preparedStmtParams = new string[1];
            preparedStmtParams[0] = barcode;
            List<BarcodeResult> res = conn.Query<BarcodeResult>("SELECT barcodeStr, name FROM BarcodeResult WHERE barcodeStr=?", preparedStmtParams);
            if (res.Count > 0)
            {
                return res.FirstOrDefault().name;
            }
            else return null;
        }

        public static void AddBarcodeResultToCache(string barcode, string result)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<BarcodeResult>();
            BarcodeResult ins = new BarcodeResult();
            ins.barcodeStr = barcode;
            ins.name = result;
            conn.InsertOrReplace(ins);
        }

        /// <summary>
        /// For caching barcodes
        /// </summary>
        public sealed class BarcodeResult
        {
            [PrimaryKey]
            public string barcodeStr { get; set; }
            public string name { get; set; }
        }
        
        // Represents food table
        public sealed class Food
        {
            public string Id { get; set; }
            public string name { get; set; }
            public string foodGroup;
            public override string ToString()
            {
                return name;
            }
        }
        /// <summary>
        /// Represents Recipe table
        /// </summary>
        public sealed class Recipe
        {
            public string Id;
            private string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public string instructions;
            public int prepTime;
            public int nPoints;
            public int nRequired;
            public float rating;
            public string imageStr;

            private string prepTimeString;

            public string PrepTimeString
            {
                get
                {
                    int hours = prepTime / 60;
                    int mins = prepTime % 60;
                    if (hours == 0)
                    {
                        return mins.ToString() + "m";
                    }
                    else
                    {
                        return hours.ToString() + "h " + mins.ToString() + "m";
                    }
                }
                set { prepTimeString = value; }
            }

            public BitmapImage image {get {
                if (imageStr == null) return null;
                // Deserialze the base64 encoded image
                byte[] imgByres = Convert.FromBase64String(imageStr);
                Stream s = new MemoryStream(imgByres);
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(s);
                return bmp;
            }}


            /// <summary>
            /// Deserializes instructions, and returns a list of instruction objects
            /// Ingredient objects within instructions only have name, quantity, 
            /// and units fields filled out
            /// </summary>
            /// <returns>A list of instructions with ingredients and actions</returns>
            public List<Instruction<Ingredient>> GetInstructions()
            {
                List<Instruction<Ingredient>> retval = new List<Instruction<Ingredient>>();
                string[] insArr = instructions.Split(new string[] {"[ins_end]"}, StringSplitOptions.RemoveEmptyEntries);
                for (int ins_idx = 0; ins_idx < insArr.Length; ins_idx++)
                {
                    string[] insPartsArr = insArr[ins_idx].Split(new string[] {"[ins]"}, StringSplitOptions.None);
                    string ingredientsStr = insPartsArr[0];
                    string actionStr = insPartsArr[1];
                    string ins_text = insPartsArr[2];

                    // Because actions comes after ingredients, add empty actions list and fill that in later
                    Instruction<Ingredient> ins = new Instruction<Ingredient>(ins_text, new List<Instruction<Ingredient>.FoodAction>());

                    // process ingredients
                    string[] ingredientsArr = ingredientsStr.Split(';');
                    for (int ingredients_idx = 0; ingredients_idx < ingredientsArr.Length; ingredients_idx++)
                    {
                        string[] ingredients_parts = ingredientsArr[ingredients_idx].Split(',');
                        if (ingredients_parts.Length < 3) continue; // skip empty ingredients
                        ins.Add(new Ingredient(ingredients_parts[0], int.Parse(ingredients_parts[1]), ingredients_parts[2], 0, false));
                    }

                    // process actions, and populate list of actions in instruction object
                    string[] actionsArr = actionStr.Split(',');
                    for (int actions_idx = 0; actions_idx < actionsArr.Length; actions_idx++)
                    {
                        if (actionsArr[actions_idx].Length < 1) continue; // skip empty actions
                        ins.Actions.Add((Instruction<Ingredient>.FoodAction)Enum.Parse(typeof(Instruction<Ingredient>.FoodAction), actionsArr[actions_idx]));
                    }
                    retval.Add(ins);
                }

                return retval;
            }
        }

        /// <summary>
        /// Represents 'connector' table for foods and recipes
        /// No real need to use it directly
        /// </summary>
        public sealed class FoodRecipe
        {
            public string Id;
            public string foodId;
            public string recipeId;
            public int points;
            public Boolean required;
            public int quantity;
        }

        /// <summary>
        /// Represents keyword table, no real need to use it
        /// </summary>
        public sealed class Keyword
        {
            public string Id;
            public string keyword;
            public string foodId;
        }

        /// <summary>
        /// Represents a potential food match (given a raw name, return food database entries)
        /// </summary>
        public class FoodMatches
        {
            private string foodName;

            public string FoodName
            {
                get { return foodName; }
                set { foodName = value; }
            }
            public string foodId;
            public int keywordCount;
            public string foodGroup;

            public override string ToString()
            {
                return foodName;
            }
        }
    }
}
