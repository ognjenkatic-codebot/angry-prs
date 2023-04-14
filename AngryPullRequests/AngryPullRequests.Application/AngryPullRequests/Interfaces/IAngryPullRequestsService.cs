using AngryPullRequests.Application.Github;
using AngryPullRequests.Domain.Entities;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Interfaces
{
    public interface IAngryPullRequestsService
    {
        Task CheckOutPullRequests(Repository repository, IPullRequestService pullRequestService);
    }
}
