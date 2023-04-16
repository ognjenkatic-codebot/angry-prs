using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Application.Slack.Formatters;
using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Services
{
    public class SlackNotifierService : IUserNotifierService
    {
        private readonly IEnumerable<ISlackMessageFormatter> messageFormatters;
        private readonly IAngryPullRequestsContext dbContext;

        public SlackNotifierService(IEnumerable<ISlackMessageFormatter> messageFormatters, IAngryPullRequestsContext dbContext)
        {
            this.messageFormatters = messageFormatters;
            this.dbContext = dbContext;
        }

        public async Task Notify(
            PullRequestNotificationGroup[] pullRequestNotificationGroups,
            Repository repository,
            IPullRequestService pullRequestService
        )
        {
            var api = new SlackServiceBuilder().UseApiToken(repository.Characteristics.SlackApiToken).GetApiClient();

            var blocks = new List<Block>();

            foreach (var formatter in messageFormatters)
            {
                blocks.AddRange(await formatter.GetBlocks(pullRequestNotificationGroups, repository));
            }

            await api.Chat.PostMessage(new Message { Blocks = blocks, Channel = repository.Characteristics.SlackNotificationChannel });
        }

        public async Task SendTestMessage(Repository repository)
        {
            var api = new SlackServiceBuilder().UseApiToken(repository.Characteristics.SlackApiToken).GetApiClient();
            var blocks = new List<Block>
            {
                new HeaderBlock
                {
                    Text = new PlainText() { Text = "Sve ok", Emoji = true }
                }
            };

            await api.Chat.PostMessage(new Message { Blocks = blocks, Channel = repository.Characteristics.SlackNotificationChannel });
        }
    }
}
