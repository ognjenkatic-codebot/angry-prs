using AngryPullRequests.Application.Slack.Formatters;
using SlackNet.Blocks;
using SlackNet.Interaction;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class AngrySlashCommandHandler : IAngrySlashCommandHandler
    {
        private readonly ForgottenPullRequestsMessageFormatter forgottenPullRequestsMessageFormatter;
        private readonly IAngryPullRequestsService angryPullRequestsService;

        public AngrySlashCommandHandler(IEnumerable<ISlackMessageFormatter> messageFormatters, IAngryPullRequestsService angryPullRequestsService)
        {
            forgottenPullRequestsMessageFormatter = messageFormatters.OfType<ForgottenPullRequestsMessageFormatter>().FirstOrDefault();
            this.angryPullRequestsService = angryPullRequestsService;
        }

        public async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            if (forgottenPullRequestsMessageFormatter != null)
            {
                var groups = await angryPullRequestsService.GetNotificationGroups();

                var blocks = new List<Block>();

                blocks.AddRange(forgottenPullRequestsMessageFormatter.GetBlocks(groups.ToArray()));

                return new SlashCommandResponse
                {
                    ResponseType = ResponseType.Ephemeral,
                    Message = new SlackNet.WebApi.Message { Blocks = blocks, Channel = command.ChannelName }
                };
            }

            return new SlashCommandResponse
            {
                ResponseType = ResponseType.Ephemeral,
                Message = new SlackNet.WebApi.Message { Text = "hello back", Channel = command.ChannelName }
            };
        }
    }
}
