using AngryPullRequests.Application.AngryPullRequests.Common.Interfaces;
using AngryPullRequests.Application.AngryPullRequests.Queries;
using AngryPullRequests.Application.Github;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    public class MyRepositoriesModel : PageModel
    {
        private readonly IMediator mediator;

        public ICollection<Repository>? Repositories { get; set; }

        public MyRepositoriesModel(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task OnGet()
        {
            Repositories = await mediator.Send(new ListRepositoriesQuery());
        }
    }
}
