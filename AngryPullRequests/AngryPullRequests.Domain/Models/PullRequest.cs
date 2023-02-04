using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class PullRequest
    {
        /// <summary>
        /// The internal Id for this pull request (not the pull request number)
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// The URL for this pull request.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// The URL for the pull request page.
        /// </summary>
        public string HtmlUrl { get; private set; }

        /// <summary>
        /// The URL for the pull request's diff (.diff) file.
        /// </summary>
        public string DiffUrl { get; private set; }

        /// <summary>
        /// The URL for the specific pull request issue.
        /// </summary>
        public string IssueUrl { get; private set; }

        /// <summary>
        /// The URL for the pull request statuses.
        /// </summary>
        public string StatusesUrl { get; private set; }

        /// <summary>
        /// The pull request number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Whether the pull request is open or closed. The default is <see cref="ItemState.Open"/>.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Title of the pull request.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The body (content) contained within the pull request.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// When the pull request was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; private set; }

        /// <summary>
        /// When the pull request was last updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; private set; }

        /// <summary>
        /// When the pull request was closed.
        /// </summary>
        public DateTimeOffset? ClosedAt { get; private set; }

        /// <summary>
        /// When the pull request was merged.
        /// </summary>
        public DateTimeOffset? MergedAt { get; private set; }

        /// <summary>
        /// The user who created the pull request.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// The user who is assigned the pull request.
        /// </summary>
        public User Assignee { get; private set; }

        /// <summary>
        ///The multiple users this pull request is assigned to.
        /// </summary>
        public IReadOnlyList<User> Assignees { get; private set; }

        /// <summary>
        /// Whether or not the pull request is in a draft state, and cannot be merged.
        /// </summary>
        public bool Draft { get; private set; }

        /// <summary>
        /// Whether or not the pull request has been merged.
        /// </summary>
        public bool Merged
        {
            get { return MergedAt.HasValue; }
        }

        /// <summary>
        /// Whether or not the pull request can be merged.
        /// </summary>
        public bool? Mergeable { get; private set; }

        /// <summary>
        /// Provides extra information regarding the mergeability of the pull request.
        /// </summary>
        public string MergeableState { get; private set; }

        /// <summary>
        /// The user who merged the pull request.
        /// </summary>
        public User MergedBy { get; private set; }

        /// <summary>
        /// The value of this field changes depending on the state of the pull request.
        /// Not Merged - the hash of the test commit used to determine mergeability.
        /// Merged with merge commit - the hash of said merge commit.
        /// Merged via squashing - the hash of the squashed commit added to the base branch.
        /// Merged via rebase - the hash of the commit that the base branch was updated to.
        /// </summary>
        public string MergeCommitSha { get; private set; }

        /// <summary>
        /// Total number of comments contained in the pull request.
        /// </summary>
        public int Comments { get; private set; }

        /// <summary>
        /// Total number of commits contained in the pull request.
        /// </summary>
        public int Commits { get; private set; }

        /// <summary>
        /// Total number of additions contained in the pull request.
        /// </summary>
        public int Additions { get; private set; }

        /// <summary>
        /// Total number of deletions contained in the pull request.
        /// </summary>
        public int Deletions { get; private set; }

        /// <summary>
        /// Total number of files changed in the pull request.
        /// </summary>
        public int ChangedFiles { get; private set; }

        /// <summary>
        /// If the issue is locked or not
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Whether maintainers of the base repository can push to the HEAD branch
        /// </summary>
        public bool? MaintainerCanModify { get; private set; }

        /// <summary>
        /// Users requested for review
        /// </summary>
        public IReadOnlyList<User> RequestedReviewers { get; private set; }


        public IReadOnlyList<Label> Labels { get; private set; }

    }
}
