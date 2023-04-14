using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Web
{
    public class AppConfiguration
    {
        public JiraConfiguration JiraConfiguration { get; set; }
        public RepositoryConfiguration RepositoryConfiguration { get; set; }
        public SlackConfiguration SlackConfiguration { get; set; }
        public SchedulingConfiguration Scheduling { get; set; }
        public PullRequestPreferences PullRequestPreferences { get; set; }
        public OpenAiConfiguration OpenAiConfiguration { get; set; }
    }
}
