using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class AngryPullRequestsService : IAngryPullRequestsService
    {
        private readonly RepositoryConfiguration configuration;
        private readonly IPullRequestService pullRequestService;
        private readonly IPullRequestStateService pullRequestStateService;
        private readonly IUserNotifierService userNotifierService;

        public AngryPullRequestsService(
            IPullRequestService pullRequestService,
            IPullRequestStateService pullRequestStateService,
            IUserNotifierService userNotifierService,
            RepositoryConfiguration configuration
        )
        {
            this.configuration = configuration;
            this.pullRequestService = pullRequestService;
            this.pullRequestStateService = pullRequestStateService;
            this.userNotifierService = userNotifierService;
        }

        public async Task CheckOutPullRequests()
        {
            var notificationGroups = await GetNotificationGroups();

            if (notificationGroups.Any())
            {
                await userNotifierService.Notify(notificationGroups.ToArray());
            }
        }

        public async Task<List<PullRequestNotificationGroup>> GetNotificationGroups()
        {
            var pullRequests = await pullRequestService.GetPullRequests(configuration.Owner, configuration.Repository, false, 1, 30, 1);

            return pullRequests.Select(async pr => await GetNotificationGroup(pr)).Select(t => t.Result).Where(ng => ng != null).ToList();
        }

        private async Task<PullRequestNotificationGroup> GetNotificationGroup(PullRequest pullRequest)
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
