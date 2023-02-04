using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using AutoMapper;
using AngryPullRequests.Infrastructure.Services;
using Octokit;
using AngryPullRequests.Infrastructure.Models;

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
            builder
                .Register(c =>
                {
                    var configuration = c.Resolve<RepositoryAccessConfiguration>();
                    var tokenAuth = new Credentials(configuration.AccessToken);
                    var client = new GitHubClient(new ProductHeaderValue("Test")) { Credentials = tokenAuth };
                    return client;
                })
                .As<IGitHubClient>();
        }
    }
}
