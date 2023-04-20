using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Domain.Entities
{
    public class RepositoryContributor
    {
        public Guid ContributorId { get; set; }
        public virtual Contributor Contributor { get; set; }
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
        public int? MergedPullRequestCount { get; set; }
        public int? ReviewedPullRequestCount { get; set; }
        public DateTimeOffset? FirstMergeAt { get; set; }
        public DateTimeOffset? LastMergeAt { get; set; }
        public DateTimeOffset? FirstReviewAt { get; set; }
        public DateTimeOffset? LastReviewAt { get; set; }
        public int CommentCount { get; set; }
        public int ApprovalCount { get; set; }
        public int ChangeRequestCount { get; set; }
    }
}
