using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Persistence
{
    public interface IAngryPullRequestsContext
    {
        DbSet<AngryUser> Users { get; set; }
        DbSet<Repository> Repositories { get; set; }
        DbSet<RunSchedule> RunSchedules { get; set; }
        DbSet<Contributor> Contributors { get; set; }
        DbSet<RepositoryContributor> RepositoryContributors { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
