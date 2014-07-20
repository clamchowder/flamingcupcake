using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace phonetest8
{
    public class FridgeFood : INotifyPropertyChanged
    {
        [PrimaryKey]
        public string foodId { get; set; }
        public string foodName { get; set; }

        public string foodGroup { get; set; }
        public int quantity { get; set; }
        public FridgeFood(string fid)
        {
            quantity = 0;
            foodId = fid;
        }
        public FridgeFood()
        {
            quantity = 0;
            foodId = "";
        }
        public FridgeFood(string name, string group)
        {
            foodName = name;
            foodGroup = group;
        }
        /// <summary>
        /// Reconstructs object from serialized representation 
        /// created by toSerializedString
        /// </summary>
        /// <param name="serializedString"></param>
        /// <param name="delim">A parameter that doesn't matter</param>
        public FridgeFood(string serializedString, char delim)
        {
            string[] parts = serializedString.Split('\x1f');
            if (parts.Count() < 4) return; // error out if parsing fails
            foodId = parts[0];
            foodName = parts[1];
            foodGroup = parts[2];
            quantity = int.Parse(parts[3]);
        }
        public override string ToString()
        {
            return foodId;
        }

        /// <summary>
        /// Used for writing object to flat file storage
        /// </summary>
        /// <returns>String representation of object</returns>
        public string toSerializedString()
        {
            string output = "";
            output += foodId + "\x1f" + foodName + "\x1f" + foodGroup + "\x1f" + quantity;
            return output;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
