using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using IssueTracker.Extensions;
using IssueTracker.Interfaces;
using IssueTracker.Model;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IssueTracker.Pages
{
    /// <summary>
    /// Interaction logic for IssuesPage.xaml
    /// </summary>
    public partial class IssuesPage : UserControl, IContent
    {
        private readonly IDataProvider _dataProvider;
        private ObservableCollection<IssueViewModel> _issueDataCollection;
        private readonly Geometry _pauseStreamGeometry = Geometry.Parse("F1 M 26.9167,23.75L 33.25,23.75L 33.25,52.25L 26.9167,52.25L 26.9167,23.75 Z M 42.75,23.75L 49.0833,23.75L 49.0833,52.25L 42.75,52.25L 42.75,23.75 Z ");
        private readonly Geometry _playStreamGeometry = Geometry.Parse("F1 M 30.0833,22.1667L 50.6665,37.6043L 50.6665,38.7918L 30.0833,53.8333L 30.0833,22.1667 Z");
        private readonly PolyStopwatch _worklogs = new PolyStopwatch();

        /// <summary>
        /// Constructor. Get and set the data provider 
        /// </summary>
        public IssuesPage()
        {
            InitializeComponent();
            _dataProvider = ((App)Application.Current).DataProvider;
        }

        //TODO: refactor to dataprovider
        private ObservableCollection<IssueViewModel> GetData()
        {
            var issues = new ObservableCollection<IssueViewModel>();

            foreach (var issue in _dataProvider.Issues)
            {
                issues.Add(new IssueViewModel
                {
                    Id = issue.Id,
                    Project = issue.Project,
                    Status = issue.Status,
                    Summary = issue.Summary,
                    Worklog = issue.Worklog,
                    CheckedItem = false,
                    IconData = _playStreamGeometry
                });
            }
            return issues;
        }
        
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dataProvider.Logon)
            {
                await RefreshMethod();
            }
            else
            {
                LogoutStateHandling();
            }
        }

        private async Task RefreshMethod()
        {
            BtnRefresh.IsEnabled = false;
            MessageToUi("Refressing...");
            ProgressBarIssuesChange.IsIndeterminate = true;

            await Task.Run(() => _dataProvider.GetIssues());

            RefreshIssues();
            BtnRefresh.IsEnabled = true;
            ProgressBarIssuesChange.IsIndeterminate = false;
            MessageToUi("Issues refresd - " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }
        
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!_dataProvider.Logon)
            {
                LogoutStateHandling();
                return;
            }

            var btn = (ModernToggleButton)sender;

            var selectedIssue = IssueDataGrid.CurrentItem as IssueViewModel;
            if (selectedIssue != null) ToggleWorkLog(btn, selectedIssue.Id);
        }

        private void ToggleWorkLog(ModernToggleButton btn, string label)
        {
            if (btn.IsChecked != null && (bool)btn.IsChecked)
            {
                var selectedIssue = _issueDataCollection.FirstOrDefault(x => x.Id == label);
                if (selectedIssue == null) return;

                SetIssuesToBasicState();
                selectedIssue.CheckedItem = true;
                selectedIssue.IconData = _pauseStreamGeometry;
                _worklogs.Start(label);
                IssueDataGrid.Items.Refresh();
            }
            else
            {
                var selectedIssue = _issueDataCollection.FirstOrDefault(x => x.Id == label);
                if (selectedIssue == null) return;

                SetIssuesToBasicState();
                _worklogs.Stop();
                IssueDataGrid.Items.Refresh();
            }
        }

        private async void BtnFlushTimes_Click(object sender, RoutedEventArgs e)
        {
            if (!_dataProvider.Logon)
            {
                LogoutStateHandling();
                return;
            }

            if (!TrackedTimeHandle()) return;

            TrackedWorksPage trackedWorksPage;
            ModernDialog uploadDialog;
            ChoosingDialodMethod(out trackedWorksPage, out uploadDialog, "Upload Tracked Logs");

            if (uploadDialog.MessageBoxResult != MessageBoxResult.Yes) return;
            _worklogs.Stop();
            SetIssuesToBasicState();

            var message = string.Empty;

            foreach (var item in trackedWorksPage.TrackedIssueDataGrid.Items)
            {
                if ((!(item is IssueTrackedViewModel)) || !(item as IssueTrackedViewModel).Selected) continue;
                //TODO: upload minutes to Jira
                var jiraIssue = item as IssueTrackedViewModel;
                _dataProvider.AddWorkLog(jiraIssue.Id,jiraIssue.TrackedMinutes);
                _worklogs.Delete((item as IssueTrackedViewModel).Id);
                message += (item as IssueTrackedViewModel).Id + " ";
            }

            if (message == string.Empty) return;
            await RefreshMethod();
        }

        private async void BtnDeleteTimes_Click(object sender, RoutedEventArgs e)
        {
            if (!TrackedTimeHandle()) return;

            TrackedWorksPage trackedWorksPage;
            ModernDialog deleteDialog;
            ChoosingDialodMethod(out trackedWorksPage, out deleteDialog, "Delete Tracked Log");

            if (deleteDialog.MessageBoxResult != MessageBoxResult.Yes) return;
            _worklogs.Stop();
            SetIssuesToBasicState();

            foreach (var item in trackedWorksPage.TrackedIssueDataGrid.Items)
            {
                if ((!(item is IssueTrackedViewModel)) || !(item as IssueTrackedViewModel).Selected) continue;
                _worklogs.Delete((item as IssueTrackedViewModel).Id);
            }

            if (_dataProvider.Logon)
                await RefreshMethod();
        }

        private bool TrackedTimeHandle()
        {
            if (_worklogs.FlushTimes().Count != 0) return true;
            MessageBoxButton button = MessageBoxButton.OK;
            ModernDialog.ShowMessage("You must track your working time first!", "No any data to handle", button);
            return false;
        }

        private void ChoosingDialodMethod(out TrackedWorksPage trackedWorksPage, out ModernDialog uploadDialog, string title)
        {
            var allTimes = _worklogs.FlushTimes();

            var trackedIssues = allTimes.Select(time => new IssueTrackedViewModel(time.Key, time.Value)).ToList();

            trackedWorksPage = new TrackedWorksPage(trackedIssues);
            uploadDialog = new ModernDialog
            {
                Title = title,
                Content = trackedWorksPage
            };
            uploadDialog.Buttons = new[] { uploadDialog.YesButton, uploadDialog.NoButton };
            uploadDialog.ShowDialog();
        }

        private static void LogoutStateHandling()
        {
            MessageBoxButton button = MessageBoxButton.OK;
            ModernDialog.ShowMessage("You must login first!", "Can't access!", button);
        }

        private void SetIssuesToBasicState()
        {
            _issueDataCollection.ToList().ForEach(x => x.CheckedItem = false);
            _issueDataCollection.ToList().ForEach(x => x.IconData = _playStreamGeometry);
        }

        private void MessageToUi(string message)
        {
            TxtInfo.Text = message;
        }

        private void RefreshIssues()
        {
            _issueDataCollection = GetData();
            IssueDataGrid.DataContext = _issueDataCollection;
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            
        }

        /// <summary>
        /// If direct navigate to this page refresh the issues
        /// </summary>
        /// <param name="e">FragmentNavigationEventArgs</param>
        public async void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            await RefreshMethod();
        }
    }

}
