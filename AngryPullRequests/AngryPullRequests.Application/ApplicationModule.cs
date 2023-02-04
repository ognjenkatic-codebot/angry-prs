using AngryPullRequests.Application.Services;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Application
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterType<RunnerService>().As<IRunnerService>();
            builder.RegisterType<AngryPullRequestsService>().As<IAngryPullRequestsService>();
            builder.RegisterType<PullRequestStateService>().As<IPullRequestStateService>();
            builder.RegisterType<SlackNotifierService>().As<IUserNotifierService>();
        }
    }
}
