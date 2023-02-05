using SlackNet.Interaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class AngrySlashCommandHandler : IAngrySlashCommandHandler
    {
        public Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            return Task.FromResult(
                new SlashCommandResponse
                {
                    ResponseType = ResponseType.InChannel,
                    Message = new SlackNet.WebApi.Message { Text = "hello back", Channel = command.ChannelName }
                }
            );
        }
    }
}
