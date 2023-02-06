using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class JiraConfiguration
    {
        public string IssueBaseUrl { get; set; }
        public string IssueRegex { get; set; }
    }
}
