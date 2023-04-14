using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class RunnerService : IRunnerService
    {
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;
        private readonly IAngryPullRequestsContext dbContext;

        public RunnerService(Func<IAngryPullRequestsService> angryPullRequestServiceFactory, IAngryPullRequestsContext dbContext)
        {
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
            this.dbContext = dbContext;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Run();
                await Task.Delay(GetDelayToNextMinute(), cancellationToken);
            }
        }

        private TimeSpan GetDelayToNextMinute()
        {
            var now = DateTimeOffset.UtcNow;
            var delay = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute + 1, 5, TimeSpan.Zero) - now;
            return delay;

            //var target = new DateTime(
            //    DateTime.Now.Year,
            //    DateTime.Now.Month,
            //    DateTime.Now.Day,
            //    schedulingConfiguration.Hour,
            //    schedulingConfiguration.Minute,
            //    0
            //);
            //var today = DateTime.Now;

            //// Check if the target time has already passed for today
            //if (today.TimeOfDay.TotalMinutes > target.TimeOfDay.TotalMinutes)
            //{
            //    // If so, calculate the time until the target time on the next work day
            //    target = target.AddDays(1);
            //    while (target.DayOfWeek == DayOfWeek.Saturday || target.DayOfWeek == DayOfWeek.Sunday)
            //    {
            //        target = target.AddDays(1);
            //    }
            //}

            //// Calculate the timespan until the target time
            //var timeUntilTarget = target - today;
            //Console.WriteLine($"Waiting for {timeUntilTarget:c} to next run");
            //return timeUntilTarget;
        }

        private async Task Run()
        {
            var repos = await dbContext.RunSchedules.Include(r => r.Repository).ToListAsync();

            foreach (var repo in repos)
            {
                Console.WriteLine(repo.Repository.Name);
            }
            //await angryPullRequestServiceFactory().CheckOutPullRequests();
        }
    }
}
