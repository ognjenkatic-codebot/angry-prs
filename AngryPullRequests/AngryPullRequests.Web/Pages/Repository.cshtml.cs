using AngryPullRequests.Application.AngryPullRequests.Commands;
using AngryPullRequests.Application.AngryPullRequests.Queries;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Octokit;
using SlackNet;
using System.ComponentModel.DataAnnotations;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    [BindProperties]
    public class RepositoryModel : PageModel
    {
        public class Model
        {
            public required Guid Id { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string Name { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string Owner { get; set; }
            public required TimeOnly TimeOfDay { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string PullRequestNameRegex { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string PullRequestNameCaptureRegex { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string ReleaseTagRegex { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string InProgressLabel { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int SmallPrChangeCount { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int LargePrChangeCount { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int OldPrAgeInDays { get; set; }
            [Required]
            [Range(1, int.MaxValue)]
            public int InactivePrAgeInDays { get; set; }
            [Required]
            [Range(0.01f, int.MaxValue)]
            public float DeleteHeavyRatio { get; set; }
            [Required]
            [StringLength(maximumLength: 150, MinimumLength = 3)]
            public required string SlackAccessToken { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string IssueBaseUrl { get; set; }
            [Required]
            [StringLength(maximumLength: 150, MinimumLength = 3)]
            public required string SlackApiToken { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string IssueRegex { get; set; }
            [Required]
            [StringLength(maximumLength: 50, MinimumLength = 3)]
            public required string SlackNotificationChannel { get; set; }
        }

        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public Model? Repository { get; set; }

        public RepositoryModel(IMediator mediator, IMapper mapper, INotyfService toastNotification)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
        }

        public async Task OnGet(Guid id)
        {
            var repository = await mediator.Send(new GetRepositoryQuery { Id = id });

            Repository = mapper.Map<Model>(repository);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var updateCommand = mapper.Map<UpdateRepositoryCommand>(Repository);

            var updatedRepo = await mediator.Send(updateCommand);

            toastNotification.Success("Repository has been saved");

            return RedirectToPage($"/Repository", updatedRepo.Id);
        }

        public async Task<IActionResult> OnPostGithubTestAsync()
        {
            if (Repository is null)
            {
                toastNotification.Error($"Unknown error", 5);
                return Page();
            }

            try
            {
                await mediator.Send(new FetchTestGithubCommand { RepositoryName = Repository.Name, RepositoryOwner = Repository.Owner });

                toastNotification.Success("Test Github request sucesfully completed");
            }
            catch (AuthorizationException)
            {
                toastNotification.Error(
                    "Authorization failed, Github token is invalid or does not have the required permissions",
                    5
                );
            }
            catch (Exception)
            {
                toastNotification.Error($"Unknown error while registering, contact administrator", 5);
            }

            return RedirectToPage($"/Repository", Repository.Id);
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (Repository is null)
            {
                toastNotification.Error($"Unknown error", 5);
                return Page();
            }

            try
            {
                await mediator.Send(new DeleteRepositoryCommand { Id = Repository.Id });
                toastNotification.Success("Repostory has been unregistered");
            }
            catch (Exception)
            {
                toastNotification.Error($"Unknown error while unregistering repository, contact administrator", 5);
            }

            return RedirectToPage("/MyRepositories");
        }


        public async Task<IActionResult> OnPostSlackTestAsync()
        {
            if (Repository is null)
            {
                toastNotification.Error($"Unknown error", 5);
                return Page();
            }

            try
            {
                await mediator.Send(
                    new SendTestSlackMessageCommand
                    {
                        ApiToken = Repository.SlackApiToken,
                        SlackNotificationChannel = Repository.SlackNotificationChannel
                    }
                );

                toastNotification.Success("Test slack message sucessfully sent");
            }
            catch (SlackException ex)
            {
                if (ex.ErrorCode == "invalid_auth")
                {
                    toastNotification.Error("Authorization failed, could not send slack message", 5);
                }
                else if (ex.ErrorCode == "channel_not_found")
                {
                    toastNotification.Error("Channel not found, could not send slack message", 5);
                }
                else
                {
                    toastNotification.Error($"Generic slack error, could not send slack message", 5);
                }
            }
            catch (Exception)
            {
                toastNotification.Error($"Unknown error while, contact administrator", 5);
            }

            return RedirectToPage($"/Repository", Repository.Id);
        }
    }
}
