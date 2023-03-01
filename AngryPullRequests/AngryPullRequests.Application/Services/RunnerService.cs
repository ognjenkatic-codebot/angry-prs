using AngryPullRequests.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;
        private readonly SchedulingConfiguration schedulingConfiguration;

        public RunnerService(Func<IAngryPullRequestsService> angryPullRequestServiceFactory, SchedulingConfiguration schedulingConfiguration)
        {
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
            this.schedulingConfiguration = schedulingConfiguration;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(GetDelayToNextRun());
                await Run();
            }
        }

        private TimeSpan GetDelayToNextRun()
        {
            return TimeSpan.FromSeconds(5);
            var target = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                schedulingConfiguration.Hour,
                schedulingConfiguration.Minute,
                0
            );
            var today = DateTime.Now;

            // Check if the target time has already passed for today
            if (today.TimeOfDay.TotalMinutes > target.TimeOfDay.TotalMinutes)
            {
                // If so, calculate the time until the target time on the next work day
                target = target.AddDays(1);
                while (target.DayOfWeek == DayOfWeek.Saturday || target.DayOfWeek == DayOfWeek.Sunday)
                {
                    target = target.AddDays(1);
                }
            }

            // Calculate the timespan until the target time
            var timeUntilTarget = target - today;
            Console.WriteLine($"Waiting for {timeUntilTarget:c} to next run");
            return timeUntilTarget;
        }

        private async Task Run()
        {
            await angryPullRequestServiceFactory().CheckOutPullRequests();
        }
    }
}
