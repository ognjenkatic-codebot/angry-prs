using AngryPullRequests.Application.AngryPullRequests.Models;
using AngryPullRequests.Domain.Entities;
using SlackNet.Blocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public interface ISlackMessageFormatter
    {
        Task<List<Block>> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups, Repository repository);
    }
}
