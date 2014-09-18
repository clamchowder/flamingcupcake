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
    public partial class EnterFoods : PhoneApplicationPage
    {
        public EnterFoods()
        {
            InitializeComponent();
        }

        private void ScanReceipt_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterFoodsOcr.xaml", UriKind.Relative));
        }

        private void ScanBarcode_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterFoodsBarcode.xaml", UriKind.Relative));
        }

        private void TypeName_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/EnterFoodsManually.xaml", UriKind.Relative));
        }
    }
}