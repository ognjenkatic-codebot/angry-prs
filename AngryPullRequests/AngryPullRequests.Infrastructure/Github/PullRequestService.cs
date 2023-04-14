using AngryPullRequests.Application.Services;
using AutoMapper;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Github
{
    public class PullRequestService : IPullRequestService
    {
        private readonly IGitHubClient gitHubClient;
        private readonly IMapper mapper;

        public PullRequestService(IGitHubClient gitHubClient, IMapper mapper)
        {
            this.gitHubClient = gitHubClient;
            this.mapper = mapper;
        }

        public async Task<Domain.Models.PullRequest[]> GetPullRequests(
            string owner,
            string repository,
            bool getAll,
            int pageCount,
            int pageSize,
            int startPage
        )
        {
            try
            {
                var pullRequests = await gitHubClient.PullRequest.GetAllForRepository(
                    owner,
                    repository,
                    new PullRequestRequest { State = getAll ? ItemStateFilter.All : ItemStateFilter.Open, SortProperty = PullRequestSort.Created },
                    new ApiOptions
                    {
                        PageCount = pageCount,
                        PageSize = pageSize,
                        StartPage = startPage
                    }
                );

                return mapper.Map<Domain.Models.PullRequest[]>(pullRequests.ToArray());
            }
            catch (Exception exc)
            {
                Console.WriteLine("ecx");
            }

            return null;
        }

        public async Task<Domain.Models.PullRequestReview[]> GetPullRequsetReviews(string owner, string repository, int pullRequestNumber)
        {
            var reviews = await gitHubClient.PullRequest.Review.GetAll(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.PullRequestReview[]>(reviews.ToArray());
        }

        public async Task<Domain.Models.User[]> GetRequestedReviewersUsers(string owner, string repository, int pullRequestNumber)
        {
            var requestedReviewersUsers = await gitHubClient.PullRequest.ReviewRequest.Get(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.User[]>(requestedReviewersUsers.Users.ToArray());
        }

        public async Task<Domain.Models.PullRequest> GetPullRequestDetails(string owner, string repository, int pullRequestNumber)
        {
            var pullRequest = await gitHubClient.PullRequest.Get(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.PullRequest>(pullRequest);
        }
    }
}
