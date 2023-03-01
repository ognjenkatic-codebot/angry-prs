using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public interface ICompletionService
    {
        public Task<string> GetCompletion(string prompt);
    }
}
