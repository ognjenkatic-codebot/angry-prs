using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Contributors;
using AngryPullRequests.Application.AngryPullRequests.Contributors.Commands;
using AngryPullRequests.Application.AngryPullRequests.Contributors.Queries;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using Autofac;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace AngryPullRequests.Web.Services
{
    public class RunnerHostedService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly ILifetimeScope lifetimeScope;
        private readonly IMediator mediator;

        public RunnerHostedService(ILifetimeScope lifetimeScope, IMediator mediator)
        {
            this.lifetimeScope = lifetimeScope;
            this.mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await mediator.Send(new IndexContributionsCommand(), cancellationToken);
            var timer = new Timer(async _ => await Run(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task Run()
        {
            using var scope = lifetimeScope.BeginLifetimeScope();

            var dbContext = scope.Resolve<IAngryPullRequestsContext>();
            var pullRequestServiceFactory = scope.Resolve<IPullRequestServiceFactory>();
            var angryPullRequestServiceFactory = scope.Resolve<Func<IAngryPullRequestsService>>();

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

                if (isNow)
                {
                    var service = await pullRequestServiceFactory.Create(repo.Repository);
                    runTasks.Add(angryPullRequestServiceFactory().CheckOutPullRequests(repo.Repository, service));
                }
            }

            await Task.WhenAll(runTasks);
        }

        public void Dispose()
        {
            _tokenSource.Dispose();
        }
    }
}