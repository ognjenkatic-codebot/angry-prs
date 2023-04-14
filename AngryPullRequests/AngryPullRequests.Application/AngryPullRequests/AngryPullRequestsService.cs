using AngryPullRequests.Application.AngryPullRequests.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Models;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests
{
    public class AngryPullRequestsService : IAngryPullRequestsService
    {
        private readonly IPullRequestServiceFactory pullRequestServiceFactory;
        private readonly IUserNotifierService userNotifierService;
        private readonly IAngryPullRequestsContext dbContext;

        public AngryPullRequestsService(
            IPullRequestServiceFactory pullRequestServiceFactory,
            IUserNotifierService userNotifierService,
            IAngryPullRequestsContext dbContext
        )
        {
            this.pullRequestServiceFactory = pullRequestServiceFactory;
            this.userNotifierService = userNotifierService;
            this.dbContext = dbContext;
        }

        public async Task CheckOutPullRequests(string repositoryName, string repositoryOwner)
        {
            var repository = await dbContext.Repositories
                .Include(r => r.Characteristics)
                .FirstAsync(r => r.Name == repositoryName && r.Owner == repositoryOwner);

            var pullRequestService = await pullRequestServiceFactory.Create(repository.Name, repository.Owner);

            var notificationGroups = await GetNotificationGroups(repository.Name, repository.Owner, repository.Characteristics, pullRequestService);

            if (notificationGroups.Any())
            {
                await userNotifierService.Notify(notificationGroups.ToArray(), repository.Name, repository.Owner);
            }
        }

        private async Task<List<PullRequestNotificationGroup>> GetNotificationGroups(
            string repositoryName,
            string repositoryOwner,
            RepositoryCharacteristics characteristics,
            IPullRequestService pullRequestService
        )
        {
            var pullRequests = await pullRequestService.GetPullRequests(repositoryOwner, repositoryName, false, 1, 30, 1);

            var tasks = pullRequests
                .Select(pr => GetNotificationGroup(pr, repositoryName, repositoryOwner, characteristics, pullRequestService))
                .ToList();

            var response = await Task.WhenAll(tasks);

            return response.Where(r => r != null).ToList();
        }

        private async Task<PullRequestNotificationGroup> GetNotificationGroup(
            PullRequest pullRequest,
            string repositoryName,
            string repositoryOwner,
            RepositoryCharacteristics characteristics,
            IPullRequestService pullRequestService
        )
        {
            var pullRequestStateService = new PullRequestStateService(characteristics);

            var requestedReviewersTask = pullRequestService.GetRequestedReviewersUsers(repositoryOwner, repositoryName, pullRequest.Number);

            var reviewsTask = pullRequestService.GetPullRequsetReviews(repositoryOwner, repositoryName, pullRequest.Number);

            var detailedPullRequestTask = pullRequestService.GetPullRequestDetails(repositoryOwner, repositoryName, pullRequest.Number);

            await Task.WhenAll(requestedReviewersTask, reviewsTask, detailedPullRequestTask);

            var isApproved = pullRequestStateService.IsPullRequestApproved(pullRequest, reviewsTask.Result, requestedReviewersTask.Result);

            if (isApproved)
            {
                return null;
            }

            return new PullRequestNotificationGroup { PullRequest = detailedPullRequestTask.Result, Reviewers = requestedReviewersTask.Result };
        }
    }
}
