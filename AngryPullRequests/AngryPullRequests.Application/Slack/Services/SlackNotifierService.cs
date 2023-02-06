using AngryPullRequests.Application.Models;
using AngryPullRequests.Application.Services;
using AngryPullRequests.Application.Slack.Formatters;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Services
{
    public class SlackNotifierService : IUserNotifierService
    {
        private readonly ISlackApiClient slack;
        private readonly IEnumerable<ISlackMessageFormatter> messageFormatters;
        private readonly SlackConfiguration slackConfiguration;

        public SlackNotifierService(
            ISlackApiClient slack,
            IEnumerable<ISlackMessageFormatter> messageFormatters,
            SlackConfiguration slackConfiguration
        )
        {
            this.slack = slack;
            this.messageFormatters = messageFormatters;
            this.slackConfiguration = slackConfiguration;
        }

        public async Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups)
        {
            foreach (var formatter in messageFormatters)
            {
                var blocks = formatter.GetBlocks(pullRequestNotificationGroups);

                await slack.Chat.PostMessage(new Message { Blocks = blocks, Channel = slackConfiguration.NotificationsChannel });
            }
        }
    }
}
