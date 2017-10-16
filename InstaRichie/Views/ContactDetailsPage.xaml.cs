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
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public ContactDetailsPage()
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
            conn.CreateTable<Models.ContactDetails>();
            var query = conn.Table<Models.ContactDetails>();
            ContactListView.ItemsSource = query.ToList();
        }

        private  async void AddContact(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameText.Text.ToString() == "" || LastNameText.Text.ToString() == "" || PhoneText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("All fields must be filled", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Models.ContactDetails()
                    {
                        FirstName = FirstNameText.Text,
                        LastName = LastNameText.Text,
                        Phone = int.Parse(PhoneText.Text)
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
                    MessageDialog dialog = new MessageDialog("A Similar Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void UpdateContact(object sender, RoutedEventArgs e)
        {

            MessageDialog ShowConf = new MessageDialog("Do you want to update your Contact Details");
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
                    string ContactFirstName = FirstNameText.Text;
                    string ContactLastName = LastNameText.Text;
                    int ContactPhone = int.Parse(PhoneText.Text);
                    int ContactID = ((Models.ContactDetails)ContactListView.SelectedItem).ID;
                    if (ContactFirstName == "")
                    {
                        MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        conn.CreateTable<Models.ContactDetails>();
                        var query1 = conn.Table<Models.ContactDetails>();
                        var query3 = conn.Query<Models.ContactDetails>("UPDATE ContactDetails SET FirstName ='"+ContactFirstName+ "', LastName ='" + ContactLastName + "', Phone ='" + ContactPhone + "' WHERE ID = '" + ContactID + "'");
                        ContactListView.ItemsSource = query1.ToList();
                    }
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
           


        }

        private async void DeleteContact(object sender, RoutedEventArgs e)
        {
            try
            {
                string ContactSelection = ((Models.ContactDetails)ContactListView.SelectedItem).FirstName;
                if (ContactSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Models.ContactDetails>();
                    var query1 = conn.Table<Models.ContactDetails>();
                    var query3 = conn.Query<Models.ContactDetails>("DELETE FROM ContactDetails WHERE FirstName ='" + ContactSelection + "'");
                    ContactListView.ItemsSource = query1.ToList();
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
            FirstNameText.Text = string.Empty;
            LastNameText.Text = string.Empty;
            PhoneText.Text = string.Empty;

            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
