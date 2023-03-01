using AngryPullRequests.Application.Services;
using AngryPullRequests.Application.Slack.Formatters;
using AngryPullRequests.Application.Slack.Services;
using AngryPullRequests.Console.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Mappings;
using AngryPullRequests.Infrastructure.Models;
using AngryPullRequests.Infrastructure.Services.Github;
using AngryPullRequests.Infrastructure.Services.OpenAi;
using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Octokit;
using SlackNet.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Console.Autofac
{
    public class ConsoleModule : Module
    {
        private readonly AppConfiguration appConfiguration;

        public ConsoleModule(AppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
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

            builder.RegisterType<RunnerService>().As<IRunnerService>();
            builder.RegisterType<AngryPullRequestsService>().As<IAngryPullRequestsService>();
            builder.RegisterType<PullRequestStateService>().As<IPullRequestStateService>();
            builder.RegisterType<SlackNotifierService>().As<IUserNotifierService>();
            builder.RegisterType<OpenAiCompletionService>().As<ICompletionService>();

            builder.RegisterType<PullRequestService>().As<IPullRequestService>();
            builder
                .Register(c =>
                {
                    var tokenAuth = new Credentials(appConfiguration.RepositoryConfiguration.AccessToken);
                    var client = new GitHubClient(new ProductHeaderValue("Test")) { Credentials = tokenAuth };
                    return client;
                })
                .As<IGitHubClient>();

            builder.AddSlackNet(
                c =>
                    c.UseApiToken(appConfiguration.SlackConfiguration.ApiToken)
                        .UseAppLevelToken(appConfiguration.SlackConfiguration.AccessToken)
                        .RegisterSlashCommandHandler<AngrySlashCommandHandler>("/apr-all")
            );

            builder.RegisterType<ForgottenPullRequestsMessageFormatter>().As<ISlackMessageFormatter>();
            builder.RegisterType<DeveloperLoadMessageFormatter>().As<ISlackMessageFormatter>();
        }
    }
}
