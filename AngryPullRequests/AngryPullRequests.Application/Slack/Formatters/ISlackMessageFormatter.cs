using AngryPullRequests.Application.Models;
using SlackNet.Blocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public interface ISlackMessageFormatter
    {
        Task<List<Block>> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups);
    }
}
