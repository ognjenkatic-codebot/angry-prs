using AngryPullRequests.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Contributors.Queries
{
    public class UserRankingStats
    {
        public string Username { get; set; }
        public int AuthoredPullRequests { get; set; }
        public DateTimeOffset LastAuthoringAt { get; set; }
        public DateTimeOffset FirstAuthoringAt { get; set; }
        public string MostActiveEverOnRepository { get; set; }
        public string RecentlyMostActiveOn { get; set; }
    }

    public class GetUserContributionsRankedQuery : IRequest<List<UserRankingStats>>
    {
        public class Handler : IRequestHandler<GetUserContributionsRankedQuery, List<UserRankingStats>>
        {
            private readonly IAngryPullRequestsContext dbContext;

            public Handler(IAngryPullRequestsContext dbContext)
            {
                this.dbContext = dbContext;
            }

            public async Task<List<UserRankingStats>> Handle(GetUserContributionsRankedQuery request, CancellationToken cancellationToken)
            {
                var contributions = await dbContext.RepositoryContributors.Include(rc => rc.Contributor).Include(rc => rc.Repository).ToListAsync();

                var response = new List<UserRankingStats>();

                foreach (var contribution in contributions)
                {
                    var userContribution = response.FirstOrDefault(uc => uc.Username == contribution.Contributor.GithubUsername);

                    if (userContribution is null)
                    {
                        response.Add(
                            new UserRankingStats
                            {
                                AuthoredPullRequests = (int)contribution.MergedPullRequestCount,
                                FirstAuthoringAt = (DateTimeOffset)contribution.FirstMergeAt,
                                LastAuthoringAt = (DateTimeOffset)contribution.LastMergeAt,
                                Username = contribution.Contributor.GithubUsername,
                                RecentlyMostActiveOn = contribution.Repository.Name
                            }
                        );
                    }
                    else
                    {
                        userContribution.AuthoredPullRequests += (int)contribution.MergedPullRequestCount;
                        userContribution.FirstAuthoringAt =
                            (DateTimeOffset)userContribution.FirstAuthoringAt < (DateTimeOffset)contribution.FirstMergeAt
                                ? (DateTimeOffset)userContribution.FirstAuthoringAt
                                : (DateTimeOffset)contribution.FirstMergeAt;

                        if (contribution.LastMergeAt > userContribution.LastAuthoringAt)
                        {
                            userContribution.LastAuthoringAt = (DateTimeOffset)contribution.LastMergeAt;
                            userContribution.RecentlyMostActiveOn = contribution.Repository.Name;
                        }
                    }
                }

                return response.OrderByDescending(r => r.AuthoredPullRequests).ThenBy(r => r.Username).ToList();
            }
        }
    }
}
