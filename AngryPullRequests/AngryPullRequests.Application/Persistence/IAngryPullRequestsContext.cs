using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Application.Persistence
{
    public interface IAngryPullRequestsContext
    {
        DbSet<AngryUser> Users { get; set; }
        DbSet<Repository> Repositories { get; set; }
        DbSet<RunSchedule> RunSchedules { get; set; }
    }
}
