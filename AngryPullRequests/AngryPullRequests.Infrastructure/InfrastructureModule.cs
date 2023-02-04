using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using AutoMapper;
using AngryPullRequests.Infrastructure.Services;
using Octokit;

namespace AngryPullRequests.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
            builder.RegisterAutoMapper(typeof(InfrastructureModule).Assembly);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<PullRequestService>().As<IPullRequestService>();
            builder.RegisterInstance(new GitHubClient(new ProductHeaderValue("Test"))).As<IGitHubClient>();
        }
    }
}
