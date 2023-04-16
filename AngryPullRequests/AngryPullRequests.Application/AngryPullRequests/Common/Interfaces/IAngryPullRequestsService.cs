using AngryPullRequests.Application.Github;
using AngryPullRequests.Domain.Entities;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Interfaces
{
    public interface IAngryPullRequestsService
    {
        Task CheckOutPullRequests(Repository repository, IPullRequestService pullRequestService);
    }
}
