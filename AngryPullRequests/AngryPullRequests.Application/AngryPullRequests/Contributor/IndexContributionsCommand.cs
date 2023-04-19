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

namespace AngryPullRequests.Application.AngryPullRequests.Contributor
{
    public class IndexContributionsCommand : IRequest
    {
        public class Handler : IRequestHandler<IndexContributionsCommand>
        {
            private readonly IAngryPullRequestsContext dbContext;
            private readonly IPullRequestServiceFactory pullRequestServiceFactory;

            private static int lastPageFetched = 1;
            private static int totalPrs = 0;

            private static Dictionary<string, UserExperience> authorExperienceMap = new Dictionary<string, UserExperience>();

            public Handler(IAngryPullRequestsContext dbContext, IPullRequestServiceFactory pullRequestServiceFactory)
            {
                this.dbContext = dbContext;
                this.pullRequestServiceFactory = pullRequestServiceFactory;
            }

            public async Task Handle(IndexContributionsCommand request, CancellationToken cancellationToken)
            {
                var repositories = await dbContext.Repositories.ToListAsync();

                foreach (var repository in repositories)
                {
                    var pullRequestService = await pullRequestServiceFactory.Create(repository);
                    var goNext = false;

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
                            ProcessPullRequest(pr);
                        }
                    } while (goNext);
                }
            }

            private void ProcessPullRequest(PullRequest pullRequest)
            {
                var author = pullRequest.User.Login;

                if (!authorExperienceMap.ContainsKey(author))
                {
                    authorExperienceMap[author] = new UserExperience();
                }

                if (pullRequest.Merged)
                {
                    authorExperienceMap[author].PullRequestsMerged++;

                    if (pullRequest.CreatedAt < authorExperienceMap[author].FirstMerge)
                    {
                        authorExperienceMap[author].FirstMerge = pullRequest.CreatedAt.UtcDateTime;
                    }
                    if (pullRequest.CreatedAt > authorExperienceMap[author].LastMerge)
                    {
                        authorExperienceMap[author].LastMerge = pullRequest.CreatedAt.UtcDateTime;
                    }
                }
            }
        }
    }
}
