using AngryPullRequests.Application.AngryPullRequests.Models;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Interfaces
{
    public interface IUserNotifierService
    {
        Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups, Repository repository, IPullRequestService pullRequestService);
    }
}
