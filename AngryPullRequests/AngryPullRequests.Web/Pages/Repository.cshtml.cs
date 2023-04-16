using AngryPullRequests.Application.AngryPullRequests.Commands;
using AngryPullRequests.Application.AngryPullRequests.Queries;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    [BindProperties]
    public class RepositoryModel : PageModel
    {
        public class Model
        {
            public Guid Id { get; set; }
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

        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public Model Repository { get; set; }

        public RepositoryModel(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
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

            return RedirectToPage($"/Repository", updatedRepo.Id);
        }
    }
}
