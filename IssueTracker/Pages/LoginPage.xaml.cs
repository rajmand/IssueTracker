﻿using FirstFloor.ModernUI.Windows.Controls;
using IssueTracker.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using static IssueTracker.Model.ButtonContents;

namespace IssueTracker.Pages
{
    //TODO: after login show logged on
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage
    {
        private readonly IDataProvider _dataProvider;

        public LoginPage()
        {
            _dataProvider = ((App) Application.Current).DataProvider;
            InitializeComponent();
            btnLogin.Content = Login.ToString();
            Loaded += window_Loaded;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            TextUsername.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string) btnLogin.Content == Login.ToString())
            {
                await LoginMethod();
            }
            else
            {
                _dataProvider.Logout();
                EnableInputs();
                btnLogin.Content = Login.ToString();
            }
        }

        private async System.Threading.Tasks.Task LoginMethod()
        {
            ProgressRingLogin.IsActive = true;
            BlockInputs();

            try
            {
                await _dataProvider.CreateJiraClient(TextUsername.Text, TextPassword.Password);
                MessageBoxButton button = MessageBoxButton.OK;
                ModernDialog.ShowMessage("You can see your issues at issue window", "Login success!", button);

                BlockInputs();
                btnLogin.Content = Logout.ToString();
                btnLogin.IsEnabled = true;

                NavigationCommands.GoToPage.Execute("/Pages/IssuesPage.xaml#refresh", this);
            }
            catch (Exception ex)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                ModernDialog.ShowMessage(ex.Message, "Error while logging!", button);
                EnableInputs();
            }
            finally
            {
                ProgressRingLogin.IsActive = false;
            }
        }

        private void EnableInputs()
        {
            btnLogin.IsEnabled = true;
            TextUsername.IsEnabled = true;
            TextPassword.IsEnabled = true;
        }

        private void BlockInputs()
        {
            btnLogin.IsEnabled = false;
            TextUsername.IsEnabled = false;
            TextPassword.IsEnabled = false;
        }
    }

}
