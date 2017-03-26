using IssueTracker.DataServices;
using IssueTracker.Interfaces;
using IssueTracker.Properties;
using System.Windows;

namespace IssueTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IDataProvider DataProvider { get; private set; }

        public App()
        {
            DataProvider = new DataProvider(Settings.Default.JiraUrl);
        }
    }
}
