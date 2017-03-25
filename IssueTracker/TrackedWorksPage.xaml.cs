using IssueTracker.Model;
using System.Collections.Generic;
using System.Windows.Controls;

namespace IssueTracker
{
    /// <summary>
    /// Interaction logic for TrackedWorksPage.xaml
    /// </summary>
    public partial class TrackedWorksPage : UserControl
    {
        public TrackedWorksPage()
        {
            InitializeComponent();
        }

        public TrackedWorksPage(List<IssueTrackedViewModel> trackedIssues)
        {
            InitializeComponent();
            trackedIssues.ForEach(x => x.Selected = true);
            TrackedIssueDataGrid.DataContext = trackedIssues;
        }


    }
}
