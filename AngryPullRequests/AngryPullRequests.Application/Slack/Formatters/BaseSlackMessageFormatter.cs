using AngryPullRequests.Application.Models;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public abstract class BaseSlackMessageFormatter : ISlackMessageFormatter
    {
        protected PlainText CreatePe(string text) => new() { Text = text, Emoji = true };

        protected Markdown CreateMd(string text) => new() { Text = text };

        public abstract List<Block> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups);
    }
}
