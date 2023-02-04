using AutoMapper;
using Octokit;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Services
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

        public async Task<Domain.Models.PullRequest[]> GetOpenPrs(string owner, string repository)
        {
            var prs = await gitHubClient.PullRequest.GetAllForRepository(owner, repository);
            var rr = await gitHubClient.PullRequest.ReviewRequest.Get(owner, repository, prs[0].Number);

            var rews = await gitHubClient.PullRequest.Review.GetAll(owner, repository, prs[0].Number);


            return mapper.Map<Domain.Models.PullRequest[]>(prs.ToArray());
        }
    }
}
