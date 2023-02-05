using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using AutoMapper;
using AngryPullRequests.Infrastructure.Services;
using Octokit;
using AngryPullRequests.Infrastructure.Models;
using SlackNet.Autofac;
using SlackNet.Events;
using AngryPullRequests.Application.Services;
using AngryPullRequests.Infrastructure.Mappings;

namespace AngryPullRequests.Console
{
    public class InfrastructureModule : Module
    {
        private readonly RepositoryAccessConfiguration repositoryAccessConfiguration;
        private readonly SlackConfiguration slackConfiguration;

        public InfrastructureModule(RepositoryAccessConfiguration repositoryAccessConfiguration, SlackConfiguration slackConfiguration)
        {
            this.repositoryAccessConfiguration = repositoryAccessConfiguration;
            this.slackConfiguration = slackConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
            builder.RegisterAutoMapper(typeof(AutoMapperProfile).Assembly);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterInstance(slackConfiguration);
            builder.RegisterType<PullRequestService>().As<IPullRequestService>();
            builder
                .Register(c =>
                {
                    var tokenAuth = new Credentials(repositoryAccessConfiguration.AccessToken);
                    var client = new GitHubClient(new ProductHeaderValue("Test")) { Credentials = tokenAuth };
                    return client;
                })
                .As<IGitHubClient>();

            builder.AddSlackNet(
                c =>
                    c.UseApiToken(slackConfiguration.ApiToken)
                        .UseAppLevelToken(slackConfiguration.AccessToken)
                        .RegisterSlashCommandHandler<AngrySlashCommandHandler>("/test")
            );
        }
    }
}
