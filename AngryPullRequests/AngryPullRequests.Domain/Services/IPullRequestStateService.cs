using AngryPullRequests.Domain.Models;

namespace AngryPullRequests.Domain.Services
{
    public interface IPullRequestStateService
    {
        bool IsPullRequestApproved(PullRequest pullRequest, PullRequestReview[] reviews, User[] requestedReviewers);
        bool IsHuge(PullRequest pullRequest);
        bool IsSmall(PullRequest pullRequest);
        bool FollowsNamingConventions(PullRequest pullRequest);

        bool HasReviewer(PullRequest pullRequest);
        bool IsOld(PullRequest pullRequest);
        bool IsInactive(PullRequest pullRequest);
        bool IsDeleteHeavy(PullRequest pullRequest);
        bool IsInProgress(PullRequest pullRequest);
        bool HasReleaseTag(PullRequest pullRequest);
        string GetReleaseTag(PullRequest pullRequest);
        bool DoesLikelyHaveConflicts(PullRequest pullRequest);
        string GetJiraTicket(PullRequest pullRequest);
    }
}
