using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Queries
{
    public class GetRepositoryQuery :  IRequest<Repository>
    {
        public required Guid Id { get; set; }
        public class Handler : IRequestHandler<GetRepositoryQuery, Repository>
        {
            private readonly IAngryPullRequestsContext _dbContext;
            private readonly IUserService userService;

            public Handler(IAngryPullRequestsContext dbContext, IUserService userService)
            {
                _dbContext = dbContext;
                this.userService = userService;
            }

            public async Task<Repository> Handle(GetRepositoryQuery request, CancellationToken cancellationToken)
            {
                var currentUser = await userService.GetCurrentUser();

                return await _dbContext.Repositories
                    .Include(r => r.RunSchedule)
                    .Include(r => r.Characteristics)
                    .FirstAsync(c => c.Id == request.Id && c.AngryUserId == currentUser.Id, cancellationToken: cancellationToken);
            }
        }
    }
}
