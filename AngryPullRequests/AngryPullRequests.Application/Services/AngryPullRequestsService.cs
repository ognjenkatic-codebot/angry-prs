﻿using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class AngryPullRequestsService : IAngryPullRequestsService
    {
        private readonly IPullRequestService pullRequestService;
        private readonly IPullRequestStateService pullRequestStateService;
        private readonly IUserNotifierService userNotifierService;
        private readonly IAngryPullRequestsContext dbContext;

        public AngryPullRequestsService(
            IPullRequestService pullRequestService,
            IPullRequestStateService pullRequestStateService,
            IUserNotifierService userNotifierService,
            IAngryPullRequestsContext dbContext
        )
        {
            this.pullRequestService = pullRequestService;
            this.pullRequestStateService = pullRequestStateService;
            this.userNotifierService = userNotifierService;
            this.dbContext = dbContext;
        }

        public async Task CheckOutPullRequests(string repositoryName, string repositoryOwner)
        {
            var repository = await dbContext.Repositories.FirstAsync(r => r.Name == repositoryName && r.Owner == repositoryOwner);

            var notificationGroups = await GetNotificationGroups(repository.Name, repository.Owner);

            if (notificationGroups.Any())
            {
                await userNotifierService.Notify(notificationGroups.ToArray());
            }
        }

        public async Task<List<PullRequestNotificationGroup>> GetNotificationGroups(string repositoryName, string repositoryOwner)
        {
            var pullRequests = await pullRequestService.GetPullRequests(repositoryOwner, repositoryName, false, 1, 30, 1);

            return pullRequests
                .Select(async pr => await GetNotificationGroup(pr, repositoryName, repositoryOwner))
                .Select(t => t.Result)
                .Where(ng => ng != null)
                .ToList();
        }

        private async Task<PullRequestNotificationGroup> GetNotificationGroup(PullRequest pullRequest, string repositoryName, string repositoryOwner)
        {
            var requestedReviewers = await pullRequestService.GetRequestedReviewersUsers(repositoryOwner, repositoryName, pullRequest.Number);

            var reviews = await pullRequestService.GetPullRequsetReviews(repositoryOwner, repositoryName, pullRequest.Number);

            var isApproved = pullRequestStateService.IsPullRequestApproved(pullRequest, reviews, requestedReviewers);

            if (isApproved)
            {
                return null;
            }

            var detailedPullRequest = await pullRequestService.GetPullRequestDetails(repositoryOwner, repositoryName, pullRequest.Number);

            return new PullRequestNotificationGroup { PullRequest = detailedPullRequest, Reviewers = requestedReviewers };
        }
    }
}
