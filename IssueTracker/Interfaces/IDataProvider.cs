using IssueTracker.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IssueTracker.Interfaces
{
    public interface IDataProvider
    {
        bool Logon { get; }
        string Username { get; }
        List<IssueViewModel> Issues{ get; }
        Task CreateJiraClient(string username, string password);
        Task GetIssues();
        void Logout();
        void AddWorkLog(string jiraId, double minutes);
        void OpenInBrowserIssueById(string jiraId);
    }
}