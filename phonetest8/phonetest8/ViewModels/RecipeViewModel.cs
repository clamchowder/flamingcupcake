using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace phonetest8
{
    public class RecipeViewModel : INotifyPropertyChanged
    {
        public static List<Ingredient> UnsortedIngredientsList;
        public bool IsDataLoaded { get; private set; }
        public static db.Recipe recipe;

        public RecipeViewModel()
        {
            UnsortedIngredientsList = new List<Ingredient>();
            this.LoadData();
        }

        /* LongListSelector binds to GroupedInstructionsList and GroupedIngredientsList
         * These are called upon from MainPage.xaml's LLS's
         */
        public List<Instruction<Ingredient>> GroupedInstructionsList
        {
            get
            {
                return CreateInstructions();
            }
        }

        public List<StringKeyGroup<Ingredient>> GroupedIngredientsList
        {
            get
            {
                return CreateGroups();
            }
        }

        // Load UnsortedIngredientsList with random fake foods for the first time
        public void LoadData()
        {
            List<Instruction<Ingredient>> insList = recipe.GetInstructions();

            /*Ingredient i1 = new Ingredient("chicken", "proteins", 2, "pounds", 1, true);
            Ingredient i2 = new Ingredient("cashews", "proteins", 3, "cups", 1, true);
            Ingredient i3 = new Ingredient("cauliflower", "produce", 1, "head", 3, true);

            UnsortedIngredientsList.Add(i1);
            UnsortedIngredientsList.Add(i2);
            UnsortedIngredientsList.Add(i3); */

            this.IsDataLoaded = true;
        }

        public class StringKeyGroup<T> : List<T>
        {
            public string Key { get; private set; }

            public StringKeyGroup(string key)
            {
                Key = key;
            }

            public void InsertOrUpdate(T ingr)
            {
                Ingredient candidate = ingr as Ingredient;
                foreach (T i in this)
                {
                    Ingredient ingredient = i as Ingredient;
                    if (ingredient.Name == candidate.Name)
                    {
                        ingredient.Quantity += candidate.Quantity;
                        return;
                    }
                }
                this.Add(ingr);
            }
        }

        public static List<StringKeyGroup<Ingredient>> CreateGroups()
        {
            List<Instruction<Ingredient>> insList = recipe.GetInstructions();

            // Create List to hold final list
            List<StringKeyGroup<Ingredient>> groupedIngredientsList = new List<StringKeyGroup<Ingredient>>();
            StringKeyGroup<Ingredient> produceIngs = new StringKeyGroup<Ingredient>("produce");
            StringKeyGroup<Ingredient> proteinsIngs = new StringKeyGroup<Ingredient>("proteins");
            StringKeyGroup<Ingredient> dairyIngs = new StringKeyGroup<Ingredient>("dairy");
            StringKeyGroup<Ingredient> grainsIngs = new StringKeyGroup<Ingredient>("grains");
            StringKeyGroup<Ingredient> condimentsIngs = new StringKeyGroup<Ingredient>("condiments");
            StringKeyGroup<Ingredient> miscIngs = new StringKeyGroup<Ingredient>("misc");         
            
            // Fill each list with the appropriate Ingredients
            foreach(Instruction<Ingredient> instruction in insList){
                foreach (Ingredient ingredient in instruction)
                {
                    if (ingredient.Foodgroup == "produce") produceIngs.InsertOrUpdate(ingredient);
                    else if (ingredient.Foodgroup == "proteins") proteinsIngs.InsertOrUpdate(ingredient);
                    else if (ingredient.Foodgroup == "dairy") dairyIngs.InsertOrUpdate(ingredient);
                    else if (ingredient.Foodgroup == "grains") grainsIngs.InsertOrUpdate(ingredient);
                    else if (ingredient.Foodgroup == "condiments") condimentsIngs.InsertOrUpdate(ingredient);
                    else miscIngs.InsertOrUpdate(ingredient);
                }
            }

            // Populate grouped list with each food group list
            groupedIngredientsList.Add(produceIngs);
            groupedIngredientsList.Add(proteinsIngs);
            groupedIngredientsList.Add(dairyIngs);
            groupedIngredientsList.Add(grainsIngs);
            groupedIngredientsList.Add(condimentsIngs);
            groupedIngredientsList.Add(miscIngs);

            // In essence we're returning a list of lists. This is the format longlistselector expects.
            return groupedIngredientsList;
        }
        public static List<Instruction<Ingredient>> CreateInstructions()
        {
            return recipe.GetInstructions();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}