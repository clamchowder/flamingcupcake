using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;

namespace phonetest8
{
    public partial class EnterFoodsManually : PhoneApplicationPage
    {
        ProgressIndicator prog;
        public static List<db.FoodMatches> rawMatches;

        // A status to display below the entry button
        // If status is set, text box should be cleared
        public static string status = "";

        /// <summary>
        /// Constructor. Clears list of matches
        /// </summary>
        public EnterFoodsManually()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            FoodInputBox.Text = "";
            // apply status if set by ChooseFoods (to give confirmation to user)
            // then clear status so it doesn't appear again next time the page is navigated to 
            if (status != "")
            {
                showToast(status);
            }
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
            StartInDeterminateProgress("trying to understand what you just typed");
            rawMatches = await db.getFoodMatches(input);
            if (rawMatches.Count == 0)  // alert user if no match is found
            {
                Dispatcher.BeginInvoke(() => /* necessary to prevent access issues */
                {
                    showToast("No food found :(");
                });
                return;
            }
            // Matches found, navigate to page that lets users pick
            StopInDeterminateProgress();
            ChooseFood.matches = rawMatches;
            ChooseFood.referrer = "EnterFoodsManually";
            NavigationService.Navigate(new Uri("/ChooseFood.xaml", UriKind.Relative));
        }

        private void showToast(String text)
        {
            ToastPrompt toast = new ToastPrompt();
            toast.Message = text;
            toast.FontSize = 24;
            toast.VerticalAlignment = VerticalAlignment.Bottom;
            toast.TextWrapping = TextWrapping.Wrap;
            toast.TextOrientation = System.Windows.Controls.Orientation.Vertical;

            toast.Show();
        }
        private void StartInDeterminateProgress(String text)
        {
            if (prog == null)
            {
                prog = new ProgressIndicator();
            }
            SystemTray.SetIsVisible(this, true);
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = text;
            SystemTray.SetProgressIndicator(this, prog);
        }
        private void StopInDeterminateProgress()
        {
            if (prog != null)
            {
                SystemTray.SetIsVisible(this, false);

                prog.IsVisible = false;
            }
        }
    }
}