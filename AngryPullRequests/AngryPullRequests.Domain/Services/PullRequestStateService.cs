using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AngryPullRequests.Domain.Services
{
    public class PullRequestStateService : IPullRequestStateService
    {
        private static readonly Regex pullRequestTitleRegex = new Regex(@"^FRINX-[0-9]+ \|.*$");
        private static readonly TimeSpan oldPullRequestAge = TimeSpan.FromDays(10);
        private static readonly TimeSpan inactivePullRequestAge = TimeSpan.FromDays(3);
        private static readonly Regex releaseTagRegex = new Regex(@"Q[0-9]+\.[0-9]+\.[0-9]+");

        private const float deleteHeavyRatio = 0.2f;

        public bool IsPullRequestApproved(PullRequest pullRequest, PullRequestReview[] reviews, User[] requestedReviewers)
        {
            if (reviews == null || reviews.Length == 0)
            {
                return false;
            }

            // all reviews are recorded, but we are only interested in ones that were done last by a user
            var lastReviews = GetLastReviews(reviews);

            // if user is in requseted reviewers list, we cannot count his previous reviews, so they need to be filtered out
            var nonReRequestedReviews = FilterNonReRequestedUserReviews(lastReviews, requestedReviewers);

            // a pull request is is approved if there is at least one latest review which approves it and there are no change requests
            return nonReRequestedReviews.All(r => r.Value.State != PullRequestReviewStates.ChangesRequested)
                && nonReRequestedReviews.Any(r => r.Value.State == PullRequestReviewStates.Approved);
        }

        private Dictionary<User, PullRequestReview> FilterNonReRequestedUserReviews(
            Dictionary<User, PullRequestReview> userReviews,
            User[] requestedReviewers
        )
        {
            return userReviews.Where(r => !requestedReviewers.Select(a => a.Id).Contains(r.Key.Id)).ToDictionary(r => r.Key, r => r.Value);
        }

        private Dictionary<User, PullRequestReview> GetLastReviews(PullRequestReview[] reviews)
        {
            return reviews
                .GroupBy(a => a.User.Id, b => b)
                .Select(group => new { ReviewerId = group.Key, Review = group.OrderBy(r => r.SubmittedAt).Last() })
                .ToDictionary(r => r.Review.User, r => r.Review);
        }

        public bool IsHuge(PullRequest pullRequest) => pullRequest.Additions + pullRequest.Deletions > 500;

        public bool FollowsNamingConventions(PullRequest pullRequest) => pullRequestTitleRegex.IsMatch(pullRequest.Title);

        public bool HasReviewer(PullRequest pullRequest) => pullRequest.RequestedReviewers.Any();

        public bool IsOld(PullRequest pullRequest)
        {
            var age = DateTimeOffset.Now - pullRequest.CreatedAt;

            return age >= oldPullRequestAge;
        }

        public bool IsSmall(PullRequest pullRequest) => pullRequest.Additions + pullRequest.Deletions <= 100;

        public bool IsInactive(PullRequest pullRequest)
        {
            var age = DateTimeOffset.Now - pullRequest.UpdatedAt;

            return age >= inactivePullRequestAge;
        }

        public bool IsDeleteHeavy(PullRequest pullRequest) => (pullRequest.Additions / (float)pullRequest.Deletions) <= deleteHeavyRatio;

        public bool IsInProgress(PullRequest pullRequest) => pullRequest.Labels.Any(l => l.Name == "in progress");

        public bool HasReleaseTag(PullRequest pullRequest) => pullRequest.Labels.Any(l => releaseTagRegex.IsMatch(l.Name));

        public string GetReleaseTag(PullRequest pullRequest) => pullRequest.Labels.First(l => releaseTagRegex.IsMatch(l.Name)).Name;
    }
}
