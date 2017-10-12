// **************************************************************************
// anno is the best programmer
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
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
            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList();
        }

        private async void AddShopping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_NameOfItem.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("You didn't enter an item name idiot!", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    double TempPriceQuoted = Convert.ToDouble(_PriceQuoted.Text);
                    conn.CreateTable<ShoppingList>();
                    conn.Insert(new ShoppingList
                    {
                        NameOfItem = _NameOfItem.Text.ToString(),
                        PriceQuoted = TempPriceQuoted,
                        ShoppingDate = _ShoppingDate.Text.ToString()
                    });
                    // Creating table
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    //MessageDialog dialog = new MessageDialog("That Shopping list item name already exist, Try Different Name.", "Oops..!");
                    MessageDialog dialog = new MessageDialog(ex.Message, "Oops..!");

                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("You have not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    var query3 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE NameOfItem ='" + AccSelection + "'");
                    ShoppingListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("You have not selected the Item", "Null Reference Exception!");
                await dialog.ShowAsync();
            }
        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure you want to update?", "Useless annoying dialog box");
            ShowConf.Commands.Add(new UICommand("Yes, Of course I want to update!!!")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("No Way, CANCEL!")
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

                    int ShoppingItemID = ((Models.ShoppingList)ShoppingListView.SelectedItem).ID;
                    string NameOfItem = _NameOfItem.Text;
                    double PriceQuoted = double.Parse(_PriceQuoted.Text);
                    string ShoppingDate = _ShoppingDate.Text;


                    if (NameOfItem == "")
                    {
                        MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        conn.CreateTable<Models.ShoppingList>();
                        var query1 = conn.Table<Models.ShoppingList>();
                        var query3 = conn.Query<Models.ShoppingList>("update ShoppingList set NameOfItem ='" + NameOfItem + "', PriceQuoted = '" + PriceQuoted + "', ShoppingDate ='" + ShoppingDate + "' where ID = '" + ShoppingItemID + "'");
                        //var query3 = conn.Query<Models.ContactDetails>("UPDATE ContactDetails SET FirstName ='" + ContactFirstName + "', LastName ='" + ContactLastName + "', Phone ='" + ContactPhone + "' WHERE ID = '" + ContactID + "'");
                        ShoppingListView.ItemsSource = query1.ToList();
                    }
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Update", "Oops..! Null Reference Exception");
                    await ClearDialog.ShowAsync();
                }
                catch (FormatException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Write your updated item in the correct format!!!", "Oops..! Format Exception");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        
    }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private void ShoppingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _ShoppingItemID.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).ID.ToString();
                _NameOfItem.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                _PriceQuoted.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).PriceQuoted.ToString();
                _ShoppingDate.Text = ((Models.ShoppingList)ShoppingListView.SelectedItem).ShoppingDate.ToString();
        }
            catch (NullReferenceException)
            {
                //Reset text fields when no item is selected
                _ShoppingItemID.Text = "";
                _NameOfItem.Text = "";
                _PriceQuoted.Text = "";
                _ShoppingDate.Text = "";
            }
}

    }
}
