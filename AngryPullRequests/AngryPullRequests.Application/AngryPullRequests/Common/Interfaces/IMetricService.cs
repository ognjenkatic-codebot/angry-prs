using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Interfaces
{
    public interface IMetricService
    {
        public Task<Dictionary<string, UserExperience>> GetAuthorExperience(string repository, string owner, string author);
    }
}
