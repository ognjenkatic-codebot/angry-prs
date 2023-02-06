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
        private readonly PullRequestPreferences pullRequestPreferences;
        private const string DirtyConstant = "dirty";

        public PullRequestStateService(PullRequestPreferences pullRequestPreferences)
        {
            this.pullRequestPreferences = pullRequestPreferences;
        }

        public bool IsPullRequestForgotten(PullRequest pullRequest, PullRequestReview[] reviews, User[] requestedReviewers)
        {
            if (reviews == null || reviews.Length == 0)
            {
                return false;
            }

            // all reviews are recorded, but we are only interested in ones that were done last by a user
            var lastReviews = GetLastReviews(reviews);

            // if user is in requseted reviewers list, we cannot count his previous reviews, so they need to be filtered out
            var nonReRequestedReviews = FilterNonReRequestedUserReviews(lastReviews, requestedReviewers);

            // a pull request is forgotten if there are no fresh reviews or the pr is inactive and there is a request for changes
            return (!nonReRequestedReviews.Any())
                || (nonReRequestedReviews.Any(r => r.Value.State == PullRequestReviewStates.ChangesRequested) && IsInactive(pullRequest));
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

        public bool IsHuge(PullRequest pullRequest) => pullRequest.Additions + pullRequest.Deletions > pullRequestPreferences.LargePrChangeCount;

        public bool FollowsNamingConventions(PullRequest pullRequest)
        {
            if (string.IsNullOrEmpty(pullRequestPreferences.NameRegex))
            {
                return true;
            }

            return new Regex(pullRequestPreferences.NameRegex).IsMatch(pullRequest.Title);
        }

        public bool HasReviewer(PullRequest pullRequest) => pullRequest.RequestedReviewers.Any();

        public bool IsOld(PullRequest pullRequest)
        {
            var age = DateTimeOffset.Now - pullRequest.CreatedAt;

            return age >= TimeSpan.FromDays(pullRequestPreferences.OldPrAgeByDays);
        }

        public bool IsSmall(PullRequest pullRequest) => pullRequest.Additions + pullRequest.Deletions <= pullRequestPreferences.SmallPrChangeCount;

        public bool IsInactive(PullRequest pullRequest)
        {
            var age = DateTimeOffset.Now - pullRequest.UpdatedAt;

            return age >= TimeSpan.FromDays(pullRequestPreferences.InactivePrAgeByDays);
        }

        public bool IsDeleteHeavy(PullRequest pullRequest) =>
            (pullRequest.Additions / (float)pullRequest.Deletions) <= pullRequestPreferences.DeleteHeavyRatio;

        public bool IsInProgress(PullRequest pullRequest)
        {
            if (string.IsNullOrEmpty(pullRequestPreferences.InProgressLabel))
            {
                return false;
            }

            return pullRequest.Labels.Any(l => pullRequestPreferences.InProgressLabel.Equals(l.Name));
        }

        public bool HasReleaseTag(PullRequest pullRequest)
        {
            if (string.IsNullOrEmpty(pullRequestPreferences.ReleaseTagRegex))
            {
                return true;
            }

            return pullRequest.Labels.Any(l => new Regex(pullRequestPreferences.ReleaseTagRegex).IsMatch(l.Name));
        }

        public string GetReleaseTag(PullRequest pullRequest)
        {
            if (!HasReleaseTag(pullRequest))
            {
                return null;
            }

            return pullRequest.Labels.First(l => new Regex(pullRequestPreferences.ReleaseTagRegex).IsMatch(l.Name)).Name;
        }

        public bool DoesLikelyHaveConflicts(PullRequest pullRequest) => DirtyConstant.Equals(pullRequest.MergeableState);
    }
}
