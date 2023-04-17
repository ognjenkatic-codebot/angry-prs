using AngryPullRequests.Application.AngryPullRequests.Common.Exceptions;
using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Commands
{
    public class FetchTestGithubCommand : IRequest
    {
        public required string RepositoryName { get; set; }
        public required string RepositoryOwner { get; set; }
        public class Handler : IRequestHandler<FetchTestGithubCommand>
        {
            private readonly IUserService userService;
            private readonly IPullRequestServiceFactory pullRequestServiceFactory;

            public Handler(IUserService userService, IPullRequestServiceFactory pullRequestServiceFactory)
            {
                this.userService = userService;
                this.pullRequestServiceFactory = pullRequestServiceFactory;
            }

            public async Task Handle(FetchTestGithubCommand request, CancellationToken cancellationToken)
            {
                var currentUser = await userService.GetCurrentUser();

                var pullRequestService = await pullRequestServiceFactory.Create(currentUser.GithubPat);

                var prs = await pullRequestService.GetPullRequests(request.RepositoryOwner, request.RepositoryName, false, 1, 1, 1);
            }
        }
    }
}
