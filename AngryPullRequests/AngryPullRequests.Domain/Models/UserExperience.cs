using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class UserExperience
    {
        public TimeSpan TimeSinceFirstMerge
        {
            get => DateTime.UtcNow - FirstMerge;
        }

        public TimeSpan TimeSinceLastMerge
        {
            get => DateTime.UtcNow - LastMerge;
        }

        public double MergesPerDay
        {
            get => PullRequestsMerged / (double)(TimeSinceFirstMerge.TotalDays > 0 ? TimeSinceFirstMerge.Days : 1);
        }

        public int PullRequestsMerged { get; set; }
        public DateTime FirstMerge { get; set; } = DateTime.MaxValue.ToUniversalTime();
        public DateTime LastMerge { get; set; } = DateTime.MinValue.ToUniversalTime();
    }
}
