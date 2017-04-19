using System.Windows.Input;
using System.Windows.Media;

namespace IssueTracker.Model
{
    public class IssueViewModel
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; }
        public string Worklog { get; set; }
        public bool CheckedItem { get; set; }
        public Geometry IconData { get; set; }
        public ICommand DataGridDoubleClick { get; set; }
}
}