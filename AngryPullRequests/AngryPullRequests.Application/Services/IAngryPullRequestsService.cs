using AngryPullRequests.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public interface IAngryPullRequestsService
    {
        Task CheckOutPullRequests();
        Task<List<PullRequestNotificationGroup>> GetNotificationGroups();
    }
}
