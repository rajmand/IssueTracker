using IssueTracker.Extensions;
using System;

namespace IssueTracker.Model
{
    public class IssueTrackedViewModel
    {
        public string Id { get; private set; }
        public double TrackedMinutes { get; private set; }
        public bool Selected { get; set; }

        public IssueTrackedViewModel()
        {
            
        }

        public IssueTrackedViewModel(string jiraId, double trackedTimeSeconds)
        {
            Id = jiraId;
            TrackedMinutes = Math.Ceiling(TimeSpanUtil.ConvertSecondsToMinutes(trackedTimeSeconds));
        }

    }
}
