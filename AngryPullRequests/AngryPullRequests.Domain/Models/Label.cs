using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Domain.Models
{
    public class Label
    {
        /// <summary>
        /// Id of the label
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Url of the label
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Name of the label
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// GraphQL Node Id
        /// </summary>
        public string NodeId { get; private set; }

        /// <summary>
        /// Color of the label
        /// </summary>
        public string Color { get; private set; }

        /// <summary>
        /// Description of the label
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Is default label
        /// </summary>
        public bool Default { get; private set; }
    }
}
