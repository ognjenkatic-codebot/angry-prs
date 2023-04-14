using AngryPullRequests.Application.AngryPullRequests;
using AngryPullRequests.Application.AngryPullRequests.Interfaces;
using AngryPullRequests.Application.Completion;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Slack.Formatters;
using AngryPullRequests.Application.Slack.Services;
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
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterAutoMapper(typeof(AutoMapperProfile).Assembly);

            builder.RegisterType<RunnerHostedService>().As<IHostedService>();
            builder.RegisterType<AngryPullRequestsService>().As<IAngryPullRequestsService>();
            builder.RegisterType<SlackNotifierService>().As<IUserNotifierService>();
            builder.RegisterType<OpenAiCompletionService>().As<ICompletionService>();
            builder.RegisterType<MetricService>().As<IMetricService>();
            builder.RegisterType<PullRequestService>().As<IPullRequestService>();

            builder.RegisterType<ForgottenPullRequestsMessageFormatter>().As<ISlackMessageFormatter>();
            builder.RegisterType<DeveloperLoadMessageFormatter>().As<ISlackMessageFormatter>();
        }
    }
}
