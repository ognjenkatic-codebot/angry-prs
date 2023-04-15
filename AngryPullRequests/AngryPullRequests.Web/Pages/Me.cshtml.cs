using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AngryPullRequests.Web.Pages
{
    [Authorize]
    public class MeModel : PageModel
    {
        public required string GithubLogin { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? GithubPat { get; set; }
        public required string GithubProfile { get; set; }
        public string? Note { get; set; }
        public string? AvatarUrl { get; set; }

        public MeModel()
        {
            
        }
        public void OnGet() {

            var ur = HttpContext.User.Identity as ClaimsIdentity;

            GithubLogin = HttpContext.User.Claims.First(c => c.Type == "Username").Value;
            AvatarUrl = HttpContext.User.Claims.First(c => c.Type == "AvatarUrl").Value;
            Name = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            GithubProfile = HttpContext.User.Claims.First(c => c.Type == "GithubProfile").Value;
        }
    }
}
