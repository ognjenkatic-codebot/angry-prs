﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Infrastructure.Models
{
    public class SlackConfiguration
    {
        public string AccessToken { get; set; }
        public string NotificationsChannel { get; set; }
        public string ApiToken { get; set; }
    }
}
