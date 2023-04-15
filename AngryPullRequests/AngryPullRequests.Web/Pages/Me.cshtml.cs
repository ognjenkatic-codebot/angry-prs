using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    
    public class MeModel : PageModel
    {
        private readonly IAngryPullRequestsContext dbContext;

        public required string GithubLogin { get; set; }
        [BindProperty]
        public string? Name { get; set; }
        public string? GithubPat { get; set; }
        public required string GithubProfile { get; set; }
        [BindProperty]
        public string? Note { get; set; }
        public string? AvatarUrl { get; set; }


        public MeModel(IAngryPullRequestsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task OnGet() {

            var ur = HttpContext.User.Identity as ClaimsIdentity;
            var username = ur.Claims.First(c => c.Type == "Username").Value;

            var user = await dbContext.Users.FirstAsync(u => u.UserName == username);

            GithubLogin = user.UserName;
            AvatarUrl = user.GithubAvatarUrl;
            Name = user.Name;
            Note = user.Note;
            GithubProfile = user.GithubProfile;
        }

        public async Task<IActionResult>  OnPostAsync()
        {
            var ur = HttpContext.User.Identity as ClaimsIdentity;
            var username = ur.Claims.First(c => c.Type == "Username").Value;

            var user = await dbContext.Users.FirstAsync(u => u.UserName == username);

            user.Note = Note;
            user.Name = Name;


            await dbContext.SaveChangesAsync(new CancellationTokenSource().Token);

            return RedirectToPage("/me");
        }
    }
}
