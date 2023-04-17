using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AngryPullRequests.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAngryPullRequestsContext dbContext;

        public UserService(IHttpContextAccessor httpContextAccessor, IAngryPullRequestsContext dbContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<AngryUser> GetCurrentUser()
        {
            var ur = httpContextAccessor.HttpContext!.User.Identity as ClaimsIdentity;

            var username = ur!.Claims.First(c => c.Type == "Username").Value;

            return await dbContext.Users.Include(u => u.Repositories).FirstAsync(u => u.UserName == username);
        }
    }
}
