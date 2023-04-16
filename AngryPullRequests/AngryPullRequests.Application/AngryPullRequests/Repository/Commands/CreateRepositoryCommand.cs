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
    public class CreateRepositoryCommand : IRequest<Repository>
    {
        public required string Name { get; set; }
        public required string Owner { get; set; }
        public required TimeOnly TimeOfDay { get; set; }
        public required string PullRequestNameRegex { get; set; }
        public required string PullRequestNameCaptureRegex { get; set; }
        public required string ReleaseTagRegex { get; set; }
        public required string InProgressLabel { get; set; }
        public required int SmallPrChangeCount { get; set; }
        public required int LargePrChangeCount { get; set; }
        public required int OldPrAgeInDays { get; set; }
        public required int InactivePrAgeInDays { get; set; }
        public required float DeleteHeavyRatio { get; set; }
        public required string SlackAccessToken { get; set; }
        public required string IssueBaseUrl { get; set; }
        public required string SlackApiToken { get; set; }
        public required string IssueRegex { get; set; }
        public required string SlackNotificationChannel { get; set; }
        public class Handler : IRequestHandler<CreateRepositoryCommand, Repository>
        {
            private readonly IAngryPullRequestsContext _dbContext;
            private readonly IMapper mapper;
            private readonly IUserService userService;
            private readonly IUserNotifierService userNotifierService;
            private readonly IPullRequestServiceFactory pullRequestServiceFactory;

            public Handler(IAngryPullRequestsContext dbContext, IMapper mapper, IUserService userService, IUserNotifierService userNotifierService, IPullRequestServiceFactory pullRequestServiceFactory)
            {
                _dbContext = dbContext;
                this.mapper = mapper;
                this.userService = userService;
                this.userNotifierService = userNotifierService;
                this.pullRequestServiceFactory = pullRequestServiceFactory;
            }

            public async Task<Repository> Handle(CreateRepositoryCommand request, CancellationToken cancellationToken)
            {
                var currentUser = await userService.GetCurrentUser();

                var repository = mapper.Map<Repository>(request);

                var pullRequestService = await pullRequestServiceFactory.Create(currentUser.GithubPat);

                var prs = await pullRequestService.GetPullRequests(repository.Owner, repository.Name, false, 1,1,1);
                await userNotifierService.SendTestMessage(repository);
                repository.AngryUser = currentUser;

                await _dbContext.Repositories.AddAsync(repository, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return repository;
            }
        }
    }
}
