using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using AngryPullRequests.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class AngryPullRequestsService : IAngryPullRequestsService
    {
        private readonly AngryPullRequestsConfiguration configuration;
        private readonly IPullRequestService pullRequestService;
        private readonly IPullRequestStateService pullRequestStateService;
        private readonly IUserNotifierService userNotifierService;

        public AngryPullRequestsService(
            IPullRequestService pullRequestService,
            IPullRequestStateService pullRequestStateService,
            IUserNotifierService userNotifierService,
            AngryPullRequestsConfiguration configuration
        )
        {
            this.configuration = configuration;
            this.pullRequestService = pullRequestService;
            this.pullRequestStateService = pullRequestStateService;
            this.userNotifierService = userNotifierService;
        }

        public async Task CheckOutPullRequests()
        {
            var pullRequests = await pullRequestService.GetPullRequests(configuration.Owner, configuration.Repository);

            var notificationGroups = pullRequests.Select(async pr => await CreateNotificationGroup(pr)).Select(t => t.Result).Where(rg => rg != null);

            if (notificationGroups.Any())
            {
                await userNotifierService.Notify(notificationGroups.ToArray());
            }
        }

        private async Task<PullRequestNotificationGroup> CreateNotificationGroup(PullRequest pullRequest)
        {
            var requestedReviewers = await pullRequestService.GetRequestedReviewersUsers(
                configuration.Owner,
                configuration.Repository,
                pullRequest.Number
            );

            var reviews = await pullRequestService.GetPullRequsetReviews(configuration.Owner, configuration.Repository, pullRequest.Number);

            var isApproved = pullRequestStateService.IsPullRequestApproved(pullRequest, reviews, requestedReviewers);

            if (isApproved)
            {
                return null;
            }

            var detailedPullRequest = await pullRequestService.GetPullRequestDetails(
                configuration.Owner,
                configuration.Repository,
                pullRequest.Number
            );

            return new PullRequestNotificationGroup { PullRequest = detailedPullRequest, Reviewers = requestedReviewers };
        }
    }
}
