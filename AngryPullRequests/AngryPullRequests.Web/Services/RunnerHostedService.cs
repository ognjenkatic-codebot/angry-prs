using AngryPullRequests.Application.Services;

namespace AngryPullRequests.Web.Services
{
    public class RunnerHostedService : RunnerService, IHostedService
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public RunnerHostedService(Func<IAngryPullRequestsService> angryPullRequestServiceFactory, IAngryPullRequestsContext dbContext)
            : base(angryPullRequestServiceFactory, dbContext) { }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Start(_tokenSource.Token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
