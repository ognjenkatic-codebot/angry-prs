using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Commands
{
    public class DeleteRepositoryCommand : IRequest
    {
        public required Guid Id { get; set; }
        public class Handler : IRequestHandler<DeleteRepositoryCommand>
        {
            private readonly IAngryPullRequestsContext _dbContext;
            private readonly IUserService userService;

            public Handler(IAngryPullRequestsContext dbContext, IUserService userService)
            {
                _dbContext = dbContext;
                this.userService = userService;
            }

            public async Task Handle(DeleteRepositoryCommand request, CancellationToken cancellationToken)
            {
                var currentUser = await userService.GetCurrentUser();

                var item = await _dbContext.Repositories.FirstAsync(r => r.Id == request.Id && r.AngryUserId == currentUser.Id, cancellationToken: cancellationToken);

                _dbContext.Repositories.Remove(item);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
