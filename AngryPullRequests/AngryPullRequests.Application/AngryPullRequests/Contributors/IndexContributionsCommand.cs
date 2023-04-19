using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Domain.Models;
using Autofac.Features.OwnedInstances;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Contributors
{
    public class IndexContributionsCommand : IRequest
    {
        public class Handler : IRequestHandler<IndexContributionsCommand>
        {
            private readonly IAngryPullRequestsContext dbContext;
            private readonly IPullRequestServiceFactory pullRequestServiceFactory;

            //private static readonly Dictionary<Guid, Dictionary<string, UserExperience>> authorExperienceMap = new();

            public Handler(IAngryPullRequestsContext dbContext, IPullRequestServiceFactory pullRequestServiceFactory)
            {
                this.dbContext = dbContext;
                this.pullRequestServiceFactory = pullRequestServiceFactory;
            }

            public async Task Handle(IndexContributionsCommand request, CancellationToken cancellationToken)
            {
                var repositories = await dbContext.Repositories.Include(r => r.AngryUser).ToListAsync();

                var tasks = new List<Task<(Guid, Dictionary<string, UserExperience>)>>();

                foreach (var repository in repositories)
                {
                    tasks.Add(IndexRepository(repository, await pullRequestServiceFactory.Create(repository)));
                }

                var dictionaries = await Task.WhenAll(tasks);

                var contributors = await dbContext.RepositoryContributors.Include(rc => rc.Repository).Include(rc => rc.Contributor).ToListAsync();

                foreach (var repositoryContributions in dictionaries)
                {
                    foreach (var contibution in repositoryContributions.Item2)
                    {
                        var contributorName = contibution.Key;
                        var repositoryId = repositoryContributions.Item1;
                        var contributingUser = contributors.Select(c => c.Contributor).FirstOrDefault(u => u.GithubUsername == contributorName);

                        if (contributingUser is null)
                        {
                            var contribution = new RepositoryContributor
                            {
                                Contributor = new Contributor { GithubUsername = contributorName, },
                                Repository = repositories.First(r => r.Id == repositoryId),
                                FirstMergeAt = contibution.Value.FirstAuthoring,
                                LastMergeAt = contibution.Value.LastAuthoring,
                                MergedPullRequestCount = contibution.Value.PullRequestsAuthored
                            };

                            contributors.Add(contribution);
                            dbContext.RepositoryContributors.Add(contribution);
                        }
                        else
                        {
                            var existingContribution = contributors.FirstOrDefault(
                                rc => rc.ContributorId == contributingUser.Id && rc.RepositoryId == repositoryId
                            );

                            if (existingContribution is null)
                            {
                                var contribution = new RepositoryContributor
                                {
                                    Contributor = contributingUser,
                                    Repository = repositories.First(r => r.Id == repositoryId),
                                    FirstMergeAt = contibution.Value.FirstAuthoring,
                                    LastMergeAt = contibution.Value.LastAuthoring,
                                    MergedPullRequestCount = contibution.Value.PullRequestsAuthored
                                };

                                contributors.Add(contribution);
                                dbContext.RepositoryContributors.Add(contribution);
                            }
                            else
                            {
                                existingContribution.FirstMergeAt = contibution.Value.FirstAuthoring;
                                existingContribution.LastMergeAt = contibution.Value.LastAuthoring;
                                existingContribution.MergedPullRequestCount = contibution.Value.PullRequestsAuthored;
                            }
                        }
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            private async Task<(Guid, Dictionary<string, UserExperience>)> IndexRepository(
                Repository repository,
                IPullRequestService pullRequestService
            )
            {
                int lastPageFetched = 1;
                int totalPrs = 0;
                Dictionary<string, UserExperience> authorExperienceMap = new();
                bool goNext;
                do
                {
                    var swatch = new Stopwatch();
                    swatch.Start();
                    var pullRequests = await pullRequestService.GetPullRequests(repository.Owner, repository.Name, true, 1, 100, lastPageFetched);
                    swatch.Stop();

                    totalPrs += pullRequests.Length;

                    if (pullRequests.Length >= 100)
                    {
                        goNext = true;
                        lastPageFetched++;
                    }
                    else
                    {
                        goNext = false;
                    }

                    foreach (var pr in pullRequests)
                    {
                        ProcessPullRequest(pr, repository.Id, authorExperienceMap);
                    }
                } while (goNext);

                return (repository.Id, authorExperienceMap);
            }

            private void ProcessPullRequest(PullRequest pullRequest, Guid repositoryId, Dictionary<string, UserExperience> authorExperienceMap)
            {
                var author = pullRequest.User.Login;

                if (pullRequest.Merged)
                {
                    if (!authorExperienceMap.ContainsKey(author))
                    {
                        authorExperienceMap[author] = new UserExperience();
                    }

                    authorExperienceMap[author].PullRequestsAuthored++;

                    if (pullRequest.CreatedAt < authorExperienceMap[author].FirstAuthoring)
                    {
                        authorExperienceMap[author].FirstAuthoring = pullRequest.CreatedAt.UtcDateTime;
                    }
                    if (pullRequest.CreatedAt > authorExperienceMap[author].LastAuthoring)
                    {
                        authorExperienceMap[author].LastAuthoring = pullRequest.CreatedAt.UtcDateTime;
                    }
                }
            }
        }
    }
}
