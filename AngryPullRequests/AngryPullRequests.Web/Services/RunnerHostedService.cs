using AngryPullRequests.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Web.Services
{
    public class RunnerHostedService : IHostedService
    {
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;
        private readonly IAngryPullRequestsContext dbContext;

        public RunnerHostedService(Func<IAngryPullRequestsService> angryPullRequestServiceFactory, IAngryPullRequestsContext dbContext)
        {
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
            this.dbContext = dbContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Run();
                await Task.Delay(GetDelayToNextMinute(), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        private static TimeSpan GetDelayToNextMinute()
        {
            var now = DateTimeOffset.UtcNow;
            var nowPlus = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute + 1, 5, TimeSpan.Zero);
            return nowPlus - now;
        }

        private async Task Run()
        {
            var now = DateTimeOffset.UtcNow;
            var repos = await dbContext.RunSchedules.Include(r => r.Repository).ToListAsync();

            foreach (var repo in repos)
            {
                var isNow = repo.TimeOfDay.Hour == now.Hour && repo.TimeOfDay.Minute == now.Minute;
                isNow = true;

                if (isNow)
                {
                    await angryPullRequestServiceFactory().CheckOutPullRequests(repo.Repository.Name, repo.Repository.Owner);
                }
            }
        }
    }
}
