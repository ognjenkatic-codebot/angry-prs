using System;

namespace AngryPullRequests.Domain.Entities
{
    public class Repository
    {
        public Guid Id { get; set; }
        public Guid AngryUserId { get; set; }
        public virtual AngryUser AngryUser { get; set; }
    }
}
