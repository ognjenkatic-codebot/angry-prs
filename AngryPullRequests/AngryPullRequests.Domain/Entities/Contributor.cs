using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Domain.Entities
{
    public class Contributor
    {
        public Guid Id { get; set; }
        public string GithubUsername { get; set; }
        public virtual ICollection<Repository> Repositories { get; set; }
    }
}
