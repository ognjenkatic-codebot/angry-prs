using AngryPullRequests.Application.AngryPullRequests.Contributors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AngryPullRequests.Web.Pages
{
    public class ContributionsModel : PageModel
    {
        private readonly IMediator mediator;

        public List<UserRankingStats>? Contributions { get; set; }

        public ContributionsModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task OnGet()
        {
            Contributions = await mediator.Send(new GetUserContributionsRankedQuery());
        }
    }
}
