using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Interfaces
{
    public interface IAngryPullRequestsService
    {
        Task CheckOutPullRequests(string repositoryName, string repositoryOwner);
    }
}
