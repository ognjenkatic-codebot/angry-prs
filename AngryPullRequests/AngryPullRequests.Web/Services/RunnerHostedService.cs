﻿using AngryPullRequests.Application.AngryPullRequests.Interfaces;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Web.Services
{
    public class RunnerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;
        private readonly IAngryPullRequestsContext dbContext;
        private readonly IPullRequestServiceFactory pullRequestServiceFactory;

        public RunnerHostedService(
            Func<IAngryPullRequestsService> angryPullRequestServiceFactory,
            IAngryPullRequestsContext dbContext,
            IPullRequestServiceFactory pullRequestServiceFactory
        )
        {
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
            this.dbContext = dbContext;
            this.pullRequestServiceFactory = pullRequestServiceFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Run();

                // Calculate delay untill next minute, assuming tasks don't run for more than one minute
                var now = DateTimeOffset.UtcNow;
                var nowPlus = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute + 1, 5, TimeSpan.Zero);

                await Task.Delay(nowPlus - now, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task Run()
        {
            var now = DateTimeOffset.UtcNow;
            var repos = await dbContext.RunSchedules
                .Include(r => r.Repository)
                .ThenInclude(r => r.AngryUser)
                .Include(r => r.Repository)
                .ThenInclude(r => r.Characteristics)
                .ToListAsync();

            var runTasks = new List<Task>();

            foreach (var repo in repos)
            {
                var isNow = repo.TimeOfDay.Hour == now.Hour && repo.TimeOfDay.Minute == now.Minute;
                isNow = true;

                if (isNow)
                {
                    var service = await pullRequestServiceFactory.Create(repo.Repository);
                    runTasks.Add(angryPullRequestServiceFactory().CheckOutPullRequests(repo.Repository, service));
                }
            }

            await Task.WhenAll(runTasks);
        }
    }
}
