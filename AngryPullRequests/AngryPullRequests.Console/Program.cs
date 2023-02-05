﻿using AngryPullRequests.Application;
using AngryPullRequests.Application.Services;
using AngryPullRequests.Console;
using AngryPullRequests.Console.Models;
using AngryPullRequests.Infrastructure.Models;
using Autofac;
using SlackNet;
using System.Text.Json;
using System.Text.Json.Serialization;

var configurationText = System.IO.File.ReadAllText("appsettings.json");

if (string.IsNullOrEmpty(configurationText))
{
    throw new ArgumentException("Application requires an appsettings.json file with appropriate config");
}

var configration = JsonSerializer.Deserialize<AppConfiguration>(configurationText);

var builder = new ContainerBuilder();

builder.RegisterModule(new InfrastructureModule(configration.AccessConfiguration, configration.SlackConfiguration));
builder.RegisterModule<ApplicationModule>();
builder.RegisterInstance(configration.Scheduling).SingleInstance();
builder.RegisterInstance(configration.RepoConfiguration).SingleInstance();
builder.RegisterInstance(configration.AccessConfiguration).SingleInstance();

var container = builder.Build();

var prService = container.Resolve<IRunnerService>();
var slack = container.Resolve<ISlackSocketModeClient>();

await slack.Connect();
var cts = new CancellationTokenSource();

await prService.Start(cts.Token);
