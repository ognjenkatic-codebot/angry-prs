using AngryPullRequests.Application.AngryPullRequests.Models;
using AngryPullRequests.Domain.Entities;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public abstract class BaseSlackMessageFormatter : ISlackMessageFormatter
    {
        protected PlainText CreatePe(string text) => new() { Text = text, Emoji = true };

        protected Markdown CreateMd(string text) => new() { Text = text };

        public abstract Task<List<Block>> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups, Repository repository);
    }
}
