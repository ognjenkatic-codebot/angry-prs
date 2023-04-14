﻿using AngryPullRequests.Application.Services;
using AngryPullRequests.Application.Slack.Formatters;
using AngryPullRequests.Application.Slack.Services;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Common;
using AngryPullRequests.Infrastructure.Github;
using AngryPullRequests.Infrastructure.OpenAi;
using AngryPullRequests.Web.Services;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Octokit;
using SlackNet.Autofac;

namespace AngryPullRequests.Web
{
    public class WebModule : Module
    {
        private readonly AppConfiguration appConfiguration;

        public WebModule(IConfiguration configuration)
        {
            appConfiguration = configuration.Get<AppConfiguration>()!;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAutoMapper(typeof(AutoMapperProfile).Assembly);

            builder.RegisterInstance(appConfiguration.Scheduling).SingleInstance();
            builder.RegisterInstance(appConfiguration.RepositoryConfiguration).SingleInstance();
            builder.RegisterInstance(appConfiguration.SlackConfiguration).SingleInstance();
            builder.RegisterInstance(appConfiguration.PullRequestPreferences ?? new PullRequestPreferences()).SingleInstance();
            builder.RegisterInstance(appConfiguration.JiraConfiguration ?? new JiraConfiguration()).SingleInstance();
            builder.RegisterInstance(appConfiguration.OpenAiConfiguration).SingleInstance();

            builder.RegisterType<RunnerHostedService>().As<IHostedService>();
            builder.RegisterType<AngryPullRequestsService>().As<IAngryPullRequestsService>();
            builder.RegisterType<PullRequestStateService>().As<IPullRequestStateService>();
            builder.RegisterType<SlackNotifierService>().As<IUserNotifierService>();
            builder.RegisterType<OpenAiCompletionService>().As<ICompletionService>();
            builder.RegisterType<MetricService>().As<IMetricService>();
            builder.RegisterType<PullRequestService>().As<IPullRequestService>();

            //builder.AddSlackNet(
            //    c =>
            //        c.UseApiToken(appConfiguration.SlackConfiguration.ApiToken)
            //            .UseAppLevelToken(appConfiguration.SlackConfiguration.AccessToken)
            //            .RegisterSlashCommandHandler<AngrySlashCommandHandler>("/apr-all")
            //);

            builder.RegisterType<ForgottenPullRequestsMessageFormatter>().As<ISlackMessageFormatter>();
            builder.RegisterType<DeveloperLoadMessageFormatter>().As<ISlackMessageFormatter>();
        }
    }
}
