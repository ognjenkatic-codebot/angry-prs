using AngryPullRequests.Application.Models;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public class DeveloperLoadMessageFormatter : BaseSlackMessageFormatter
    {
        public override Task<List<Block>> GetBlocks(
            PullRequestNotificationGroup[] pullRequestNotificationGroups,
            string repositoryName,
            string repositoryOwner
        )
        {
            var prsByUser = pullRequestNotificationGroups
                .SelectMany(prg => prg.Reviewers)
                .GroupBy(r => r.Login)
                .Select(group => new { Login = group.Key, Count = group.Count() })
                .OrderByDescending(pu => pu.Count);

            var userBlocks = new List<Block> { new HeaderBlock { Text = CreatePe($"Opterecenje po developeru :face_with_peeking_eye:") } };

            foreach (var user in prsByUser)
            {
                var emoji = user.Count switch
                {
                    1 => ":slightly_smiling_face:",
                    <= 3 => ":neutral_face:",
                    <= 5 => ":nauseated_face:",
                    > 5 => ":face_vomiting:"
                };

                userBlocks.Add(new ContextBlock { Elements = { CreatePe($"{user.Login}: {emoji} {user.Count}") } });
            }
            return Task.FromResult(userBlocks);
        }
    }
}
