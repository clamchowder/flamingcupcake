// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

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
    public class FridgeViewModel : INotifyPropertyChanged
    {
        private List<FridgeFood> _unsortedBatch;

        public static StringKeyGroup<FridgeFood> produceIngs;
        public static StringKeyGroup<FridgeFood> proteinsIngs;
        public static StringKeyGroup<FridgeFood> dairyIngs;
        public static StringKeyGroup<FridgeFood> grainsIngs;
        public static StringKeyGroup<FridgeFood> condimentsIngs;
        public static StringKeyGroup<FridgeFood> miscIngs;

        public FridgeViewModel()
        {
            this.UnsortedBatch = new List<FridgeFood>();
            this.LoadData();
        }

        /// <summary>
        /// A collection for FridgeFood objects.
        /// </summary>
        public List<FridgeFood> UnsortedBatch
        {
            get
            {
                return _unsortedBatch;
            }
            private set
            {
                _unsortedBatch = value;
                NotifyPropertyChanged();
            }
        }

        public List<StringKeyGroup<FridgeFood>> FridgeSortedBatch
        {
            get
            {
                App.fridgeViewModel.LoadData();
                return CreateGroups(UnsortedBatch);
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few FridgeFood objects into the Items collection.
        /// </summary>
        public void LoadData()
        {

            List<FridgeFood> foods = db.GetFridgeFoods();
            this.UnsortedBatch = new List<FridgeFood>();
            foreach (FridgeFood food in foods) this.UnsortedBatch.Add(food);

            this.IsDataLoaded = true;
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
        public static List<StringKeyGroup<FridgeFood>> CreateGroups(IEnumerable<FridgeFood> UnsortedBatch)
        {
            // Create List to hold final list
            List<StringKeyGroup<FridgeFood>> SortedBatch = new List<StringKeyGroup<FridgeFood>>();
            produceIngs = new StringKeyGroup<FridgeFood>("produce");
            proteinsIngs = new StringKeyGroup<FridgeFood>("proteins");
            dairyIngs = new StringKeyGroup<FridgeFood>("dairy");
            grainsIngs = new StringKeyGroup<FridgeFood>("grains");
            condimentsIngs = new StringKeyGroup<FridgeFood>("condiments");
            miscIngs = new StringKeyGroup<FridgeFood>("misc");

            // Fill each list with the appropriate FridgeFoods
            foreach (FridgeFood i in UnsortedBatch)
            {
                if (i.foodGroup == "produce") produceIngs.Add(i);
                else if (i.foodGroup == "proteins") proteinsIngs.Add(i);
                else if (i.foodGroup == "dairy") dairyIngs.Add(i);
                else if (i.foodGroup == "grains") grainsIngs.Add(i);
                else if (i.foodGroup == "condiments") condimentsIngs.Add(i);
                else miscIngs.Add(i);
            }

            // Add each TimeKeyGroup to the overall list
            SortedBatch.Add(produceIngs);
            SortedBatch.Add(proteinsIngs);
            SortedBatch.Add(dairyIngs);
            SortedBatch.Add(grainsIngs);
            SortedBatch.Add(condimentsIngs);
            SortedBatch.Add(miscIngs);
            //Debug.WriteLine(SortedBatch.Count);
            // In essence we're returning a list of lists. This is the format longlistselector expects.
            return SortedBatch;
        }
    }
}