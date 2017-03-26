using IssueTracker.DataServices;
using IssueTracker.Interfaces;
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
            //TODO:Get the URL from configuration
            DataProvider = new DataProvider("https://cellum.atlassian.net");
        }
    }
}
