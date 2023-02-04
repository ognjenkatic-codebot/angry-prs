using AngryPullRequests.Domain.Models;
using SlackNet;
using System;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class SlackNotifierService : IUserNotifierService
    {
        private readonly ISlackApiClient slack;

        public SlackNotifierService(ISlackApiClient slack)
        {
            this.slack = slack;
        }

        public async Task Notify(Domain.Models.User reviewer, PullRequest pullRequest)
        {
            Console.WriteLine($"Sending slack message to user {reviewer.Login}");

            var nesto = await slack.Users.GetPresence();
        }
    }
}
