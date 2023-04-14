using AngryPullRequests.Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Github
{
    public class PullRequestService : IPullRequestService
    {
        private readonly IMapper mapper;
        private readonly IAngryPullRequestsContext dbContext;

        public PullRequestService(IMapper mapper, IAngryPullRequestsContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
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
                var dbRepository = await dbContext.Repositories.Include(r => r.AngryUser).FirstAsync(r => r.Name == repository && r.Owner == owner);

                var gitHubClient = GetClient(dbRepository.AngryUser.UserName, dbRepository.AngryUser.GithubPat);

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
                Console.WriteLine(exc.Message);
            }

            return null;
        }

        public async Task<Domain.Models.PullRequestReview[]> GetPullRequsetReviews(string owner, string repository, int pullRequestNumber)
        {
            var dbRepository = await dbContext.Repositories.Include(r => r.AngryUser).FirstAsync(r => r.Name == repository && r.Owner == owner);

            var gitHubClient = GetClient(dbRepository.AngryUser.UserName, dbRepository.AngryUser.GithubPat);

            var reviews = await gitHubClient.PullRequest.Review.GetAll(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.PullRequestReview[]>(reviews.ToArray());
        }

        public async Task<Domain.Models.User[]> GetRequestedReviewersUsers(string owner, string repository, int pullRequestNumber)
        {
            var dbRepository = await dbContext.Repositories.Include(r => r.AngryUser).FirstAsync(r => r.Name == repository && r.Owner == owner);

            var gitHubClient = GetClient(dbRepository.AngryUser.UserName, dbRepository.AngryUser.GithubPat);

            var requestedReviewersUsers = await gitHubClient.PullRequest.ReviewRequest.Get(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.User[]>(requestedReviewersUsers.Users.ToArray());
        }

        public async Task<Domain.Models.PullRequest> GetPullRequestDetails(string owner, string repository, int pullRequestNumber)
        {
            var dbRepository = await dbContext.Repositories.Include(r => r.AngryUser).FirstAsync(r => r.Name == repository && r.Owner == owner);

            var gitHubClient = GetClient(dbRepository.AngryUser.UserName, dbRepository.AngryUser.GithubPat);

            var pullRequest = await gitHubClient.PullRequest.Get(owner, repository, pullRequestNumber);

            return mapper.Map<Domain.Models.PullRequest>(pullRequest);
        }

        private static GitHubClient GetClient(string username, string accessToken)
        {
            return new GitHubClient(new ProductHeaderValue("AngryPullRequests")) { Credentials = new Credentials(username, accessToken) };
        }
    }
}
