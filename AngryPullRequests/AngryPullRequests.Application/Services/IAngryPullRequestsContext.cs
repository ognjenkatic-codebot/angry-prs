using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Application.Services
{
    public interface IAngryPullRequestsContext
    {
        DbSet<AngryUser> Users { get; set; }
        DbSet<Repository> Repositories { get; set; }
    }
}
