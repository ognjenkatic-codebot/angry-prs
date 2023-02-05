using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Services
{
    public interface IPullRequestService
    {
        Task<PullRequest[]> GetPullRequests(string owner, string repository);
        Task<User[]> GetRequestedReviewersUsers(string owner, string repository, int pullRequestNumber);
        Task<PullRequestReview[]> GetPullRequsetReviews(string owner, string repository, int pullRequestNumber);
        Task<PullRequest> GetPullRequestDetails(string owner, string repository, int pullRequestNumber);
    }
}
