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
        private readonly JiraConfiguration jiraConfiguration;
        private const string DirtyConstant = "dirty";

        public PullRequestStateService(PullRequestPreferences pullRequestPreferences, JiraConfiguration jiraConfiguration)
        {
            this.pullRequestPreferences = pullRequestPreferences;
            this.jiraConfiguration = jiraConfiguration;
        }

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

        public bool HasJiraTicket(PullRequest pullRequest)
        {
            if (string.IsNullOrEmpty(jiraConfiguration.IssueRegex))
            {
                return false;
            }

            var match = new Regex(jiraConfiguration.IssueRegex).Match(pullRequest.Title);

            if (match.Success && match.Groups.Count == 2)
            {
                return true;
            }

            return false;
        }

        public string GetJiraTicket(PullRequest pullRequest)
        {
            if (!HasJiraTicket(pullRequest))
            {
                return null;
            }

            var match = new Regex(jiraConfiguration.IssueRegex).Match(pullRequest.Title);

            var value = match.Groups[1].Value.Trim();

            return value;
        }

        public bool DoesLikelyHaveConflicts(PullRequest pullRequest) => DirtyConstant.Equals(pullRequest.MergeableState);

        public string GetNameWithoutJiraTicket(PullRequest pullRequest)
        {
            if (string.IsNullOrEmpty(pullRequestPreferences.NameCaptureRegex))
            {
                return null;
            }

            if (!HasJiraTicket(pullRequest))
            {
                return null;
            }

            var match = new Regex(pullRequestPreferences.NameCaptureRegex).Match(pullRequest.Title);

            if (!match.Success || match.Groups.Count != 2)
            {
                return null;
            }

            var value = match.Groups[1].Value.Trim();

            return value;
        }
    }
}
