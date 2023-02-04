using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public interface IRunnerService
    {
        Task Start(CancellationToken cancellationToken);
    }
}
