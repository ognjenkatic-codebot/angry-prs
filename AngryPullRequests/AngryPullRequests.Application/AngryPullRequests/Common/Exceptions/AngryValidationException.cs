using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.AngryPullRequests.Common.Exceptions
{
    public abstract class AngryValidationException : Exception
    {
        public AngryValidationException(string message) : base(message) { }
    }

    public class RepositoryExists : AngryValidationException
    {
        public RepositoryExists(string message) : base(message) { }
    }

    public class UnableToAccessRepository : AngryValidationException
    {
        public UnableToAccessRepository(string message) : base(message) { }
    }

    public class UnableToSendSlackMessage : AngryValidationException
    {
        public UnableToSendSlackMessage(string message) : base(message) { }
    }
}
