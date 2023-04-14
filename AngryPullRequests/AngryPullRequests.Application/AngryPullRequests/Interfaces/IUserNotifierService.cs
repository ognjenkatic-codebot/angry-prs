using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Interfaces
{
    public interface IUserNotifierService
    {
        Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups, string repositoryName, string repositoryOwner);
    }
}
