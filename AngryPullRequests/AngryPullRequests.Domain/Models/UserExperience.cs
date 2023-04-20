using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class UserExperience
    {
        public TimeSpan TimeSinceFirstAuthoring
        {
            get => DateTime.UtcNow - FirstAuthoring;
        }

        public TimeSpan TimeSinceLastAuthoring
        {
            get => DateTime.UtcNow - LastAuthoring;
        }

        public double AuthoringsPerDay
        {
            get => PullRequestsAuthored / (double)(TimeSinceFirstAuthoring.TotalDays > 0 ? TimeSinceFirstAuthoring.Days : 1);
        }

        public int PullRequestsAuthored { get; set; }
        public DateTime FirstAuthoring { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public DateTime LastAuthoring { get; set; } = DateTime.MinValue.ToUniversalTime();
        public int Comments { get; set; }
        public int Approvals { get; set; }
        public int ChangeRequests { get; set; }
    }
}
