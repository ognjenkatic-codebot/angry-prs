using System;
using System.Collections;
using System.Collections.Generic;

namespace AngryPullRequests.Domain.Entities
{
    public class AngryUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string GithubPat { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string GithubAvatarUrl { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
