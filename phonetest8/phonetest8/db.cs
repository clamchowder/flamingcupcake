using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeView;
using SQLite;

namespace phonetest8
{
    public static class db
    {

        /// <summary>
        /// Gets a list of matching foods, given a real life food name
        /// </summary>
        /// <param name="name">'Raw' name of the food</param>
        /// <returns>await on it for a List of FoodMatches objects</returns>
        public async static Task<List<FoodMatches>> getFoodMatches(string name)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();
            _params.Add("name", name);
            var result = await App.MobileService.InvokeApiAsync<List<db.FoodMatches>>("testapi", System.Net.Http.HttpMethod.Get, _params);
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
            var result = await App.MobileService.InvokeApiAsync<List<Ingredient>>("getingredients", System.Net.Http.HttpMethod.Get, _params);
            return result;
        }
        /// <summary>
        /// Fetches list of foods in fridge, and returns all recipes
        /// that can be made with what's in the fridge
        /// </summary>
        /// <returns></returns>
        public async static Task<List<Recipe>> FindRecipes()
        {
            List<Fridge> fridgeFoods = GetFridgeFoods();
            Dictionary<string, string> _params = new Dictionary<string, string>();
            Fridge[] fridgeFoodsArr = fridgeFoods.ToArray();
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
            var result = await App.MobileService.InvokeApiAsync<List<db.Recipe>>("findrecipe", System.Net.Http.HttpMethod.Get, _params);
            return result;
        }
        /// <summary>
        /// Adds a food to the local fridge database
        /// </summary>
        /// <param name="food">FoodMatch object returned from search (or just populate one yourself with food name and id)</param>
        public static void AddFridgeFood(FoodMatches food)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<Fridge>(); // SHould do create if not exists, so stuff doesn't get overwritten
            Fridge ins = new Fridge();
            ins.foodId = food.foodId;
            ins.foodName = food.foodName;
            conn.Insert(ins);
        }
        /// <summary>
        /// Deletes the fridge item with the given id (id of fridge object)
        /// </summary>
        /// <param name="id">ID of fridge object</param>
        public static void DeleteFridgeFood(int id)
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            conn.Delete(id);
        }
        /// <summary>
        /// Fetches all objects in fridge for local display
        /// </summary>
        /// <returns>A list of Fridge objects</returns>
        public static List<Fridge> GetFridgeFoods()
        {
            SQLiteConnection conn = new SQLiteConnection("test.db", SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            return conn.Table<Fridge>().ToList<Fridge>();
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
            public string name;
            public string instructions;
            public int prepTime;
            public int nPoints;
            public int nRequired;

            /// <summary>
            /// Deserializes instructions, and returns a list of instruction objects
            /// Ingredient objects within instructions only have name, quantity, 
            /// and units fields filled out
            /// </summary>
            /// <returns>A list of instructions with ingredients and actions</returns>
            public List<Instruction> GetInstructions()
            {
                List<Instruction> retval = new List<Instruction>();
                string[] insArr = instructions.Split(new string[] {"[ins_end]"}, StringSplitOptions.RemoveEmptyEntries);
                for (int ins_idx = 0; ins_idx < insArr.Length; ins_idx++)
                {
                    Instruction ins = new Instruction();
                    string[] insPartsArr = insArr[ins_idx].Split(new string[] {"[ins]"}, StringSplitOptions.None);
                    string ingredientsStr = insPartsArr[0];
                    string actionStr = insPartsArr[1];
                    ins.text = insPartsArr[2];

                    // process ingredients
                    ins.ingredients = new List<Ingredient>();
                    string[] ingredientsArr = ingredientsStr.Split(';');
                    for (int ingredients_idx = 0; ingredients_idx < ingredientsArr.Length; ingredients_idx++)
                    {
                        string[] ingredients_parts = ingredientsArr[ingredients_idx].Split(',');
                        if (ingredients_parts.Length < 3) continue; // skip empty ingredients
                        ins.ingredients.Add(new Ingredient(ingredients_parts[0], int.Parse(ingredients_parts[1]), ingredients_parts[2], 0, false));
                    }

                    // process actions
                    ins.actions = new List<Instruction.FoodAction>();
                    string[] actionsArr = actionStr.Split(',');
                    for (int actions_idx = 0; actions_idx < actionsArr.Length; actions_idx++)
                    {
                        if (actionsArr[actions_idx].Length < 1) continue; // skip empty actions
                        ins.actions.Add((Instruction.FoodAction)Enum.Parse(typeof(Instruction.FoodAction), actionsArr[actions_idx]));
                    }
                    retval.Add(ins);
                }

                return retval;
            }
        }

        /// <summary>
        /// Represents an instruction in a recipe. A recipe
        /// object should have a list of these
        /// </summary>
        public sealed class Instruction
        {
            public enum FoodAction
            {
                dice, cut, chop, mince, mix, bake, cook, broil, boil, destroy
            }
            public string text;
            public List<Ingredient> ingredients;
            public List<FoodAction> actions;
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
        /// Represents locally stored (SQLite) fridge object
        /// </summary>
        public sealed class Fridge
        {
            public string foodId { get; set; }
            public string foodName { get; set; }
            public int quantity { get; set; } 
            public Fridge(string fid)
            {
                quantity = 0;
                foodId = fid;
            }
            public Fridge()
            {
                quantity = 0;
                foodId = "";
            }
            public override string ToString()
            {
                return foodId;
            }
        }

        /// <summary>
        /// Represents a potential food match (given a raw name, return food database entries)
        /// </summary>
        public sealed class FoodMatches
        {
            public string foodName;
            public string foodId;
            public int keywordCount;
        }
    }
}
