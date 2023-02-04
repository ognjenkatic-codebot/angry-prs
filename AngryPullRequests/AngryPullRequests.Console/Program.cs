// See https://aka.ms/new-console-template for more information
using AngryPullRequests.Infrastructure;
using AngryPullRequests.Infrastructure.Services;
using Autofac;
using Octokit;

var builder = new ContainerBuilder();

builder.RegisterModule<InfrastructureModule>();

var container = builder.Build();

var prService = container.Resolve<IPullRequestService>();

var prs = await prService.GetOpenPrs("Codaxy", "conductor-sharp");

foreach(var pr in prs)
{
    Console.WriteLine(pr.Url);
    Console.WriteLine("---------------");
    Console.WriteLine($"{pr.CreatedAt}/{pr.User.Login} | {pr.RequestedReviewers.Count}, {pr.Title}");
}