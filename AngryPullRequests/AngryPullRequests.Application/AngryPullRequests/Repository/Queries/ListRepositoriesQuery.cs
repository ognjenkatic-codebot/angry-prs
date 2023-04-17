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
        public bool GetAll { get; set; }

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
                var query = _dbContext.Repositories.Include(r => r.RunSchedule).Include(r => r.AngryUser).AsQueryable();

                if (!request.GetAll)
                {
                    var user = await userService.GetCurrentUser();
                    query = query.Where(r => r.AngryUserId == user.Id);
                }

                return await query.ToListAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
