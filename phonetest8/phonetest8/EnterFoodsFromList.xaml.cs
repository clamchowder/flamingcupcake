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
    public partial class EnterFoodsFromList : PhoneApplicationPage
    {
        /* populated by OCR page when it finds matches */
        public static List<db.FoodMatches> matches;
        public List<db.FoodMatches> FoodChoices
        {
            get
            {
                return matches;
            }
        }
        public EnterFoodsFromList()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void ingListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
        }
    }
}