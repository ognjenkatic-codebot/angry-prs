using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Domain.Entities
{
    public class RepositoryCharacteristics
    {
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
        public string PullRequestNameRegex { get; set; }
        public string PullRequestNameCaptureRegex { get; set; }
        public string ReleaseTagRegex { get; set; }
        public string InProgressLabel { get; set; }
        public int SmallPrChangeCount { get; set; }
        public int LargePrChangeCount { get; set; }
        public int OldPrAgeInDays { get; set; }
        public int InactivePrAgeInDays { get; set; }
        public float DeleteHeavyRatio { get; set; }
        public string SlackNotificationChannel { get; set; }
        public string SlackApiToken { get; set; }
        public string IssueBaseUrl { get; set; }
        public string IssueRegex { get; set; }
    }
}
