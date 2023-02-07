using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class PullRequestPreferences
    {
        public string NameCaptureRegex { get; set; }
        public string NameRegex { get; set; }
        public string ReleaseTagRegex { get; set; }
        public string InProgressLabel { get; set; }
        public int SmallPrChangeCount { get; set; } = 100;
        public int LargePrChangeCount { get; set; } = 500;
        public int OldPrAgeByDays { get; set; } = 10;
        public int InactivePrAgeByDays { get; set; } = 3;
        public float DeleteHeavyRatio { get; set; } = 0.2f;
    }
}
