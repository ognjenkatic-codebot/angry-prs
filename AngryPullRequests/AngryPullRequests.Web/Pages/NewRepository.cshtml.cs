using AngryPullRequests.Application.AngryPullRequests.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    [BindProperties]
    public class NewRepositoryModel : PageModel
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

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

        public NewRepositoryModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var createCommand = mapper.Map<CreateRepositoryCommand>(Repository);

            await mediator.Send(createCommand);

            return RedirectToPage($"/MyRepositories");
        }
    }
}
