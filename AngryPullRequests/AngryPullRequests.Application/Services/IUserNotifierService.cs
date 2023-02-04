using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public interface IUserNotifierService
    {
        Task Notify(User reviewer);
    }
}
