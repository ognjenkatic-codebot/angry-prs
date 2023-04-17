using AngryPullRequests.Application.AngryPullRequests.Commands;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using SlackNet;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    [BindProperties]
    public class NewRepositoryModel : PageModel
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public class Model
        {
            public string Name { get; set; }
            public string Owner { get; set; }
            public TimeOnly TimeOfDay { get; set; }
            public string PullRequestNameRegex { get; set; }
            public string PullRequestNameCaptureRegex { get; set; }
            public string ReleaseTagRegex { get; set; }
            public string InProgressLabel { get; set; }
            public int SmallPrChangeCount { get; set; }
            public int LargePrChangeCount { get; set; }
            public int OldPrAgeInDays { get; set; }
            public int InactivePrAgeInDays { get; set; }
            public float DeleteHeavyRatio { get; set; }
            public string SlackAccessToken { get; set; }
            public string IssueBaseUrl { get; set; }
            public string SlackApiToken { get; set; }
            public string IssueRegex { get; set; }
            public string SlackNotificationChannel { get; set; }
        }

        public Model Repository { get; set; } = new Model();

        public NewRepositoryModel(IMediator mediator, IMapper mapper, INotyfService toastNotification)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var createCommand = mapper.Map<CreateRepositoryCommand>(Repository);

            await mediator.Send(createCommand);

            return RedirectToPage($"/MyRepositories");
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

            return RedirectToPage($"/NewRepository");
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

            return RedirectToPage($"/NewRepository");
        }
    }
}
