﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class Account
    {
        /// <summary>
        /// URL of the account's avatar.
        /// </summary>
        public string AvatarUrl { get; protected set; }

        /// <summary>
        /// The account's bio.
        /// </summary>
        public string Bio { get; protected set; }

        /// <summary>
        /// URL of the account's blog.
        /// </summary>
        public string Blog { get; protected set; }

        /// <summary>
        /// Number of collaborators the account has.
        /// </summary>
        public int? Collaborators { get; protected set; }

        /// <summary>
        /// Company the account works for.
        /// </summary>
        public string Company { get; protected set; }

        /// <summary>
        /// Date the account was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; protected set; }

        /// <summary>
        /// Amount of disk space the account is using.
        /// </summary>
        public int? DiskUsage { get; protected set; }

        /// <summary>
        /// The account's email.
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// Number of followers the account has.
        /// </summary>
        public int Followers { get; protected set; }

        /// <summary>
        /// Number of other users the account is following.
        /// </summary>
        public int Following { get; protected set; }

        /// <summary>
        /// Indicates whether the account is currently hireable.
        /// </summary>
        /// <value>True if the account is hireable; otherwise, false.</value>
        public bool? Hireable { get; protected set; }

        /// <summary>
        /// The HTML URL for the account on github.com (or GitHub Enterprise).
        /// </summary>
        public string HtmlUrl { get; protected set; }

        /// <summary>
        /// The account's system-wide unique Id.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// GraphQL Node Id
        /// </summary>
        public string NodeId { get; protected set; }

        /// <summary>
        /// The account's geographic location.
        /// </summary>
        public string Location { get; protected set; }

        /// <summary>
        /// The account's login.
        /// </summary>
        public string Login { get; protected set; }

        /// <summary>
        /// The account's full name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Number of private repos owned by the account.
        /// </summary>
        public int OwnedPrivateRepos { get; protected set; }


        /// <summary>
        /// Number of private gists the account has created.
        /// </summary>
        public int? PrivateGists { get; protected set; }

        /// <summary>
        /// Number of public gists the account has created.
        /// </summary>
        public int PublicGists { get; protected set; }

        /// <summary>
        /// Number of public repos the account owns.
        /// </summary>
        public int PublicRepos { get; protected set; }

        /// <summary>
        /// Total number of private repos the account owns.
        /// </summary>
        public int TotalPrivateRepos { get; protected set; }

        /// <summary>
        /// The account's API URL.
        /// </summary>
        public string Url { get; protected set; }
    }
}
