using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;

        public RunnerService(Func<IAngryPullRequestsService> angryPullRequestServiceFactory)
        {
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Run();
                await Task.Delay(5000);
            }
        }

        private async Task Run()
        {
            await angryPullRequestServiceFactory().CheckOutPullRequests();
        }
    }
}
