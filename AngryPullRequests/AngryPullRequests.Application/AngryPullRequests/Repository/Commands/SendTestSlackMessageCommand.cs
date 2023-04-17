using AngryPullRequests.Application.AngryPullRequests.Common.Exceptions;
using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Commands
{
    public class SendTestSlackMessageCommand : IRequest
    {
        public required string ApiToken { get; set; }
        public required string SlackNotificationChannel { get; set; }

        public class Handler : IRequestHandler<SendTestSlackMessageCommand>
        {
            private readonly IUserNotifierService userNotifierService;

            public Handler(IUserNotifierService userNotifierService)
            {
                this.userNotifierService = userNotifierService;
            }

            public async Task Handle(SendTestSlackMessageCommand request, CancellationToken cancellationToken)
            {
                await userNotifierService.SendTestMessage(request.ApiToken, request.SlackNotificationChannel, "Test poruka");
            }
        }
    }
}
