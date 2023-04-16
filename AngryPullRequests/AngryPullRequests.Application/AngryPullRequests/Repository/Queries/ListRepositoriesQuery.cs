using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Common.Models;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Slack.Webhooks.Blocks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Queries
{
    public class ListRepositoriesQuery : IRequest<List<Repository>>
    {
        public class Handler : IRequestHandler<ListRepositoriesQuery, List<Repository>>
        {
            private readonly IAngryPullRequestsContext _dbContext;
            private readonly IUserService userService;

            public Handler(IAngryPullRequestsContext dbContext, IUserService userService)
            {
                _dbContext = dbContext;
                this.userService = userService;
            }

            public async Task<List<Repository>> Handle(ListRepositoriesQuery request, CancellationToken cancellationToken)
            {
                var user = await userService.GetCurrentUser();

                return await _dbContext.Repositories.Where(r => r.AngryUserId == user.Id).ToListAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
