using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Models
{
    public class PullRequestNotificationGroup
    {
        public PullRequest PullRequest { get; set; }
        public User[] Reviewers { get; set; }
    }
}
