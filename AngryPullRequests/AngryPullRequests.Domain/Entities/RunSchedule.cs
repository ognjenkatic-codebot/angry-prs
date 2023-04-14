using System;

namespace AngryPullRequests.Domain.Entities
{
    public class RunSchedule
    {
        public Guid RepositoryId { get; set; }
        public TimeOnly TimeOfDay { get; set; }
        public int[] DaysOfWeek { get; set; }
        public virtual Repository Repository { get; set; }
    }
}
