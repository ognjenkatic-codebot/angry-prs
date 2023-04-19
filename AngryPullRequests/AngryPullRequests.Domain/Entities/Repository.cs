using System;
using System.Collections;
using System.Collections.Generic;

namespace AngryPullRequests.Domain.Entities
{
    public class Repository
    {
        public Guid Id { get; set; }
        public Guid AngryUserId { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public virtual AngryUser AngryUser { get; set; }
        public virtual RunSchedule RunSchedule { get; set; }
        public virtual RepositoryCharacteristics Characteristics { get; set; }
        public virtual ICollection<RepositoryContributor> Contributions { get; set; }
    }
}
