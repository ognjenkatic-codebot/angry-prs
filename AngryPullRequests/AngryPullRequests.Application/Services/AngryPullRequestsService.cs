using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using AngryPullRequests.Infrastructure.Services;
using System;
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

            foreach (var pullRequest in pullRequests)
            {
                await CheckOutPullRequest(pullRequest);
            }
        }

        private async Task CheckOutPullRequest(PullRequest pullRequest)
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
                return;
            }

            foreach (var user in requestedReviewers)
            {
                await userNotifierService.Notify(user, pullRequest);
            }
        }
    }
}
