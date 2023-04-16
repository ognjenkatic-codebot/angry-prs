using AngryPullRequests.Domain.Entities;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Interfaces
{
    public interface IUserService
    {
        Task<AngryUser> GetCurrentUser();
    }
}
