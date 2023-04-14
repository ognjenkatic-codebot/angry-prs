using AngryPullRequests.Application.AngryPullRequests.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Models;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Application.Slack.Formatters;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEnumerable<ISlackMessageFormatter> messageFormatters;
        private readonly IAngryPullRequestsContext dbContext;

        public SlackNotifierService(IEnumerable<ISlackMessageFormatter> messageFormatters, IAngryPullRequestsContext dbContext)
        {
            this.messageFormatters = messageFormatters;
            this.dbContext = dbContext;
        }

        public async Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups, string repositoryName, string repositoryOwner)
        {
            var dbRepository = await dbContext.Repositories
                .Include(r => r.Characteristics)
                .FirstAsync(r => r.Name == repositoryName && r.Owner == repositoryOwner);

            var api = new SlackServiceBuilder().UseApiToken(dbRepository.Characteristics.SlackApiToken).GetApiClient();

            foreach (var formatter in messageFormatters)
            {
                await api.Chat.PostMessage(
                    new Message
                    {
                        Blocks = await formatter.GetBlocks(pullRequestNotificationGroups, dbRepository),
                        Channel = dbRepository.Characteristics.SlackNotificationChannel
                    }
                );
            }
        }
    }
}
