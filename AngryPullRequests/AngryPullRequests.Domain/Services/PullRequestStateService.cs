using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryPullRequests.Domain.Services
{
    public class PullRequestStateService : IPullRequestStateService
    {
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
    }
}
