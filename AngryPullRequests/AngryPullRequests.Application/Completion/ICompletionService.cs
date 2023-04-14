using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Completion
{
    public interface ICompletionService
    {
        public Task<string> GetCompletion(string prompt);
    }
}
