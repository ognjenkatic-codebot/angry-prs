using AngryPullRequests.Domain.Models;

namespace AngryPullRequests.Domain.Services
{
    public interface IPullRequestStateService
    {
        bool IsPullRequestApproved(PullRequest pullRequest, PullRequestReview[] reviews, User[] requestedReviewers);
    }
}
