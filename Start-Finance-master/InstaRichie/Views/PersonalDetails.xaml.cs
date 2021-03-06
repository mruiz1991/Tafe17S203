﻿using System;
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
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalDetails : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalDetails()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

             public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();

            /// Refresh Data
            var query = conn.Table<PersonalInfo>();
            PersonalInfoListView.ItemsSource = query.ToList();
        }

        private async void AddData(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameTxtBx.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No First Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new Models.PersonalInfo()
                    {
                        FirstName = FirstNameTxtBx.Text,
                        LastName = LastNameTxtBx.Text,
                        DOB = DOBCal.Text,
                        Gender = GenderTxtBx.Text,
                        Email = EmailTxtBx.Text,
                        Phone = PhoneTxtBx.Text

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
                    MessageDialog dialog = new MessageDialog("A Similar Personal Info record already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTxtBx.Text = string.Empty;
            LastNameTxtBx.Text = string.Empty;
            DOBCal.Text = string.Empty;
            GenderTxtBx.Text = string.Empty;
            EmailTxtBx.Text = string.Empty;
            PhoneTxtBx.Text = string.Empty;


            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteAccout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((Models.PersonalInfo)PersonalInfoListView.SelectedItem).FirstName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName ='" + AccSelection + "'");
                    PersonalInfoListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditData(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure you want to update the selected record", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Update")
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
                    string PersonalFirstName = FirstNameTxtBx.Text;
                    string PersonalLastName = LastNameTxtBx.Text;
                    string PersonalDOB = DOBCal.Text;
                    string PersonalGender = GenderTxtBx.Text;
                    string PersonalEmail = EmailTxtBx.Text;
                    string PersonalPhone = PhoneTxtBx.Text;
                    int PersonalID = ((Models.PersonalInfo)PersonalInfoListView.SelectedItem).ID;

                    if (PersonalFirstName == "")
                    {
                        MessageDialog dialog = new MessageDialog("Nothing selected", "Select an item");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        conn.CreateTable<Models.PersonalInfo>();
                        var query1 = conn.Table<Models.PersonalInfo>();
                        var query3 = conn.Query<Models.PersonalInfo>("UPDATE PersonalInfo SET FirstName ='" + PersonalFirstName + "', LastName ='" + PersonalLastName + "', DOB ='" + PersonalDOB + "', Gender ='" + PersonalGender + "', Email ='" + PersonalEmail + "', Phone ='" + PersonalPhone + "' WHERE ID = '" + PersonalID + "'");
                        PersonalInfoListView.ItemsSource = query1.ToList();
                    }
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select item to update", "Opps..");
                    await ClearDialog.ShowAsync();
                    
                }
                 
            }
                

            /*FirstNameTxtBx.Text = string.Empty;
            LastNameTxtBx.Text = string.Empty;
            DOBCal.Text = string.Empty;
            GenderTxtBx.Text = string.Empty;
            EmailTxtBx.Text = string.Empty;
            PhoneTxtBx.Text = string.Empty;*/
        }
    }
}