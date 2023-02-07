using AngryPullRequests.Application.Models;
using SlackNet.Blocks;
using System.Collections.Generic;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public interface ISlackMessageFormatter
    {
        List<Block> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups);
    }
}
