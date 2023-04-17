using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using Autofac;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Web.Services
{
    public class RunnerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly ILifetimeScope lifetimeScope;

        public RunnerHostedService(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Run();

                // Calculate delay untill next minute, assuming tasks don't run for more than one minute
                var now = DateTimeOffset.UtcNow;
                var nowPlus = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, TimeSpan.Zero).AddMinutes(1).AddSeconds(5);

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
                //isNow = true;

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
