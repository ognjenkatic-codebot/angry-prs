using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Services
{
    public interface IPullRequestService
    {
        public Task<PullRequest[]> GetOpenPrs(string owner, string repository);
    }
}
