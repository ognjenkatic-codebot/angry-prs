using SlackNet;
using SlackNet.Events;
using System;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class SlackEventHandler : IEventHandler<MessageEvent>
    {
        private readonly ISlackApiClient slack;

        public SlackEventHandler(ISlackApiClient slack)
        {
            this.slack = slack;
        }

        public Task Handle(MessageEvent slackEvent)
        {
            throw new NotImplementedException();
        }
    }
}
