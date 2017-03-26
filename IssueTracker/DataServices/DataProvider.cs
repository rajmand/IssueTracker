using Atlassian.Jira;
using FirstFloor.ModernUI.Windows.Controls;
using IssueTracker.Interfaces;
using IssueTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;

namespace IssueTracker.DataServices
{
    public class DataProvider : IDataProvider
    {
        public string Username { get; private set; }
        private string _userPassword;
        private readonly string _jiraUrl;
        private Jira _jira;
        public List<IssueViewModel> Issues { get; private set; }
        public bool Logon { get; private set; }
        public JiraUser JiraUser { get; private set; }

        public DataProvider(string url)
        {
            Logon = false;
            Issues = new List<IssueViewModel>();
            _jiraUrl = url;
        }

        //TODO: move this to jira client factory
        public async Task CreateJiraClient(string username, string password)
        {
            Username = username;
            _userPassword = password;
            _jira = Jira.CreateRestClient(_jiraUrl, Username, _userPassword);
            JiraUser = await _jira.Users.GetUserAsync(username);
            if (JiraUser != null)
            {
                Logon = true;
            }

        }

        public void Logout()
        {
            _jira = null;
            Logon = false;
            JiraUser = null;
            Issues = new List<IssueViewModel>();
            Username = string.Empty;
            _userPassword = string.Empty;
        }

        public async Task GetIssues()
        {
            try
            {
                Issues = new List<IssueViewModel>();

                if (_jira == null)
                {
                    throw new AuthenticationException("No logged users! Please try login first.");
                }

                var querydIssueList = Task.Run(() => IssueQuery().ToList());
                await querydIssueList;
                

                foreach (var issue in querydIssueList.Result)
                {
                    var project = issue.Project;
                    var id = issue.Key.Value;
                    var summary = issue.Summary;
                    var status = issue.Status.Name;
                    var worklog = issue.GetTimeTrackingDataAsync().Result.TimeSpent;
                    Issues.Add(new IssueViewModel
                    {
                        Project = project,
                        Id = id,
                        Summary = summary,
                        Status = status,
                        Worklog = worklog
                    });
                }

            }
            catch (Exception e)
            {
                //TODO: show correct error message
                MessageBoxButton button = MessageBoxButton.OK;
                ModernDialog.ShowMessage(e.Message, "Error while getting issues!", button);
            }
        }

        private IOrderedQueryable<Issue> IssueQuery()
        {
            return from i in _jira.Issues.Queryable
                                  where i.Assignee == Username
                                  orderby i.Updated
                                  select i;
        }

        public async void AddWorkLog(string jiraId, double minutes)
        {
            var issue = await _jira.Issues.GetIssueAsync(jiraId);
            await issue.AddWorklogAsync(WorkLogStrategy(minutes));
        }
        private static string WorkLogStrategy(double minutes)
        {
            return minutes + "m";
        }
    }
}
