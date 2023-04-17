using AngryPullRequests.Application.AngryPullRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AngryPullRequests.Web.Pages
{
    public class RepositoryRow
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string AngryUser { get; set; }
        public TimeOnly TimeOfDay { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly IMediator mediator;

        public ICollection<RepositoryRow>? Repositories { get; set; }

        public IndexModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task OnGet()
        {
            var repositories = await mediator.Send(new ListRepositoriesQuery { GetAll = true });

            Repositories = repositories
                .Select(
                    r =>
                        new RepositoryRow
                        {
                            Name = r.Name,
                            Owner = r.Owner,
                            AngryUser = r.AngryUser.Name,
                            TimeOfDay = r.RunSchedule.TimeOfDay
                        }
                )
                .ToList();
        }
    }
}
