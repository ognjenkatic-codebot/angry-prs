using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public interface ISlackMessageFormatter
    {
        List<Block> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups);
    }
}
