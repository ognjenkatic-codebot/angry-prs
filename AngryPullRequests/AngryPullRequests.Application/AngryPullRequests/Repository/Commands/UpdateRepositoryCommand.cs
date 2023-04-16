using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Commands
{
    public class UpdateRepositoryCommand:  IRequest<Repository>
    {
        public required Guid Id { get; set; }
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
        public class Handler : IRequestHandler<UpdateRepositoryCommand, Repository>
        {
            private readonly IAngryPullRequestsContext _dbContext;
            private readonly IUserService userService;
            private readonly IMapper mapper;

            public Handler(IAngryPullRequestsContext dbContext, IUserService userService, IMapper mapper)
            {
                _dbContext = dbContext;
                this.userService = userService;
                this.mapper = mapper;
            }

            public async Task<Repository> Handle(UpdateRepositoryCommand request, CancellationToken cancellationToken)
            {
                var currentUser = await userService.GetCurrentUser();

                var dbRepository = await _dbContext.Repositories
                    .Include(r => r.RunSchedule)
                    .Include(r => r.Characteristics)
                    .FirstAsync(
                    r => r.Id == request.Id && r.AngryUserId == currentUser.Id,
                    cancellationToken: cancellationToken
                );

                var repository = mapper.Map(request, dbRepository);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return repository;
            }
        }
    }
}
