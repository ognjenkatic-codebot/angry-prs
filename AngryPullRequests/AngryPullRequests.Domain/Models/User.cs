using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class User : Account
    {
        /// <summary>
        /// Whether or not the user is an administrator of the site
        /// </summary>
        public bool SiteAdmin { get; private set; }

        /// <summary>
        /// When the user was suspended, if at all (GitHub Enterprise)
        /// </summary>
        public DateTimeOffset? SuspendedAt { get; private set; }

        /// <summary>
        /// Whether or not the user is currently suspended
        /// </summary>
        public bool Suspended { get { return SuspendedAt.HasValue; } }

        /// <summary>
        /// Date the user account was updated.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; private set; }
    }
}
