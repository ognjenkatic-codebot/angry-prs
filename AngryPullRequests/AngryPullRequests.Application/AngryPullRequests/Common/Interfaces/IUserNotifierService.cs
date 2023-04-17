using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Interfaces
{
    public interface IUserNotifierService
    {
        Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups, Repository repository, IPullRequestService pullRequestService);
        Task SendTestMessage(string apiToken, string notificationChannel, string messageContent);
    }
}
