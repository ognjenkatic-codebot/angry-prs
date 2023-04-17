using AngryPullRequests.Application.AngryPullRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AngryPullRequests.Web.Pages
{
    public class RepositoryRow
    {
        public required string Name { get; set; }
        public required string Owner { get; set; }
        public required string AngryUser { get; set; }
        public required TimeOnly TimeOfDay { get; set; }
        public required string UserAvatar { get; set; }
        public required string UserGithubProfile { get; set; }
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
                            TimeOfDay = r.RunSchedule.TimeOfDay,
                            UserAvatar = r.AngryUser.GithubAvatarUrl,
                            UserGithubProfile = r.AngryUser.GithubProfile
                        }
                )
                .ToList();
        }
    }
}
