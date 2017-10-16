using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }
        public void Results()
        {
            // Creating table
            conn.CreateTable<Models.Appointments>();
            var query = conn.Table<Models.Appointments>();
            AppointmentListView.ItemsSource = query.ToList();
        }
        private async void AddAppointment(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AptDescriptionText.Text.ToString() == "" || AptDateText.Text.ToString() == "" || AptStartTimeText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("All fields must be filled", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (AptEndTimeText.Text == "")
                {
                    int et = int.Parse(AptStartTimeText.Text) + 20;
                    AptEndTimeText.Text = et.ToString();
                }
                else 
                {
                    conn.Insert(new Models.Appointments()
                    {

                        AptDescription = AptDescriptionText.Text,
                        AptDate = AptDateText.Text,
                        AptStartTime = int.Parse(AptStartTimeText.Text),
                        AptEndTime = int.Parse(AptEndTimeText.Text)
                        
                    });
                    
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar appointmnet already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void UpdateAppointment(object sender, RoutedEventArgs e)
        {

            MessageDialog ShowConf = new MessageDialog("Do you want to update your Appointment Details");
            ShowConf.Commands.Add(new UICommand("Yes, Update!")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    string AptDescription = AptDescriptionText.Text;
                    string AptDate = AptDateText.Text;
                    int AptStartTime = int.Parse(AptStartTimeText.Text);
                    int AptEndTime = int.Parse(AptEndTimeText.Text);
                   
                    int AptID = ((Models.Appointments)AppointmentListView.SelectedItem).ID;
                    if (AptDescription == "")
                    {
                        MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        conn.CreateTable<Models.Appointments>();
                        var query1 = conn.Table<Models.Appointments>();
                        var query3 = conn.Query<Models.Appointments>("UPDATE Appointments SET AptDescription ='" + AptDescription + "', AptDate ='" + AptDate + "', AptStartTime ='" + AptStartTime + "', AptEndTime ='" + AptEndTime + "' WHERE ID = '" + AptID + "'");
                        AppointmentListView.ItemsSource = query1.ToList();
                    }
                   
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to update", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }



        }

        private async void DeleteAppointment(object sender, RoutedEventArgs e)
        {
            try
            {
                string AppointmentsSelection = ((Models.Appointments)AppointmentListView.SelectedItem).AptDescription;
                if (AppointmentsSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Models.Appointments>();
                    var query1 = conn.Table<Models.Appointments>();
                    var query3 = conn.Query<Models.Appointments>("DELETE FROM Appointments WHERE AptDescription ='" + AppointmentsSelection + "'");
                    AppointmentListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }

        }

        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            AptDescriptionText.Text = string.Empty;
            AptDateText.Text = string.Empty;
            AptStartTimeText.Text = string.Empty;
            AptEndTimeText.Text = string.Empty;

            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
