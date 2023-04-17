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

        public Model Repository { get; set; }

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

            toastNotification.Success("A success for christian-schou.dk");

            return RedirectToPage($"/Repository", updatedRepo.Id);
        }

        public async Task<IActionResult> OnPostGithubTestAsync()
        {
            try
            {
                await mediator.Send(new FetchTestGithubCommand { RepositoryName = Repository.Name, RepositoryOwner = Repository.Owner });

                toastNotification.Success("Test Github request je uspjesno izvrsen");
            }
            catch (AuthorizationException)
            {
                toastNotification.Error(
                    "Autorizacija neuspjesna, Github token vjerovatno nije vazeci ili nema odgovarajuce permisije za ovaj repository",
                    5
                );
            }
            catch (Exception ex)
            {
                toastNotification.Error($"Nepoznata greska prilikom test github zahtjeva '{ex.Message}', kontaktirajte administratora", 5);
            }

            return RedirectToPage($"/Repository", Repository.Id);
        }

        public async Task<IActionResult> OnPostSlackTestAsync()
        {
            try
            {
                await mediator.Send(
                    new SendTestSlackMessageCommand
                    {
                        ApiToken = Repository.SlackApiToken,
                        SlackNotificationChannel = Repository.SlackNotificationChannel
                    }
                );

                toastNotification.Success("Test slack poruka je uspjesno poslana");
            }
            catch (SlackException ex)
            {
                if (ex.ErrorCode == "invalid_auth")
                {
                    toastNotification.Error("Autorizacija neuspjesna, slanje test slack poruke nije uspjelo", 5);
                }
                else if (ex.ErrorCode == "channel_not_found")
                {
                    toastNotification.Error("Kanal ne postoji, slanje test slack poruke nije uspjelo", 5);
                }
                else
                {
                    toastNotification.Error($"Genericka slack greska '{ex.ErrorCode}', slanje test slack poruke nije uspjelo", 5);
                }
            }
            catch (Exception ex)
            {
                toastNotification.Error($"Nepoznata greska prilikom test slack zahtjeva '{ex.Message}', kontaktirajte administratora", 5);
            }

            return RedirectToPage($"/Repository", Repository.Id);
        }
    }
}
