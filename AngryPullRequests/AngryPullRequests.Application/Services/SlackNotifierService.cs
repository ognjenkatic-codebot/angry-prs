using AngryPullRequests.Domain.Models;
using System;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class SlackNotifierService : IUserNotifierService
    {
        public Task Notify(User reviewer, PullRequest pullRequest)
        {
            Console.WriteLine($"Sending slack message to user {reviewer.Login}");

            return Task.CompletedTask;
        }
    }
}
