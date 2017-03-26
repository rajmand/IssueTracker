using FirstFloor.ModernUI.Windows.Controls;
using IssueTracker.Interfaces;
using System;
using System.Windows;
using System.Windows.Input;
using static IssueTracker.Model.ButtonContents;

namespace IssueTracker.Pages
{
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
            BtnLogin.Content = Login.ToString();
            Loaded += window_Loaded;
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            TextUsername.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string) BtnLogin.Content == Login.ToString())
            {
                await LoginMethod();
            }
            else
            {
                _dataProvider.Logout();
                EnableInputs();
                BtnLogin.Content = Login.ToString();
            }
        }

        private async System.Threading.Tasks.Task LoginMethod()
        {
            if (!InputValidation()) return;

            ProgressRingLogin.IsActive = true;
            BlockInputs();

            try
            {
                await _dataProvider.CreateJiraClient(TextUsername.Text, TextPassword.Password);
                MessageBoxButton button = MessageBoxButton.OK;
                ModernDialog.ShowMessage("You can see your issues at issue window", "Login success!", button);

                BlockInputs();
                BtnLogin.Content = Logout.ToString();
                BtnLogin.IsEnabled = true;

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

        private bool InputValidation()
        {
            if (TextUsername.Text == string.Empty || TextPassword.Password == string.Empty)
            {
                MessageBoxButton button = MessageBoxButton.OK;
                ModernDialog.ShowMessage("Please fill username and password", "Login fail!", button);
                return false;
            }
            return true;
        }

        private void EnableInputs()
        {
            BtnLogin.IsEnabled = true;
            TextUsername.IsEnabled = true;
            TextPassword.IsEnabled = true;
        }

        private void BlockInputs()
        {
            BtnLogin.IsEnabled = false;
            TextUsername.IsEnabled = false;
            TextPassword.IsEnabled = false;
        }
    }

}
