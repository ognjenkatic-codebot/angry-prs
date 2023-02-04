using AngryPullRequests.Application.Services;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Console
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<RunnerService>().As<IRunnerService>();
            builder.RegisterType<AngryPullRequestsService>().As<IAngryPullRequestsService>();
            builder.RegisterType<PullRequestStateService>().As<IPullRequestStateService>();
            builder.RegisterType<SlackNotifierService>().As<IUserNotifierService>();
        }
    }
}
