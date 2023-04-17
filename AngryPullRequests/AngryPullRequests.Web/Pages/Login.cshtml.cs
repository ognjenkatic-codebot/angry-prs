using AngryPullRequests.Application.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System.Security.Claims;

namespace AngryPullRequests.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAngryPullRequestsContext dbContext;

        public LoginModel(IAngryPullRequestsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect("/");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string pat, string returnUrl)
        {
            try
            {
                if (HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    return LocalRedirect("/");
                }

                var gitHubClient = new GitHubClient(new ProductHeaderValue("AngryPullRequests")) { Credentials = new Credentials(pat) };

                var pt = await gitHubClient.User.Current();

                var username = pt.Login;
                var avatarUrl = pt.AvatarUrl ?? "";
                var githubProfile = pt.HtmlUrl;
                var name = pt.Name ?? "";

                var claims = new List<Claim>
                {
                    new Claim("AvatarUrl", avatarUrl),
                    new Claim("Username", username),
                    new Claim("GithubProfile", githubProfile),
                    new Claim(ClaimTypes.Name, name)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A
                    // value set here overrides the ExpireTimeSpan option of
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = "/"
                    // The full path or absolute URI to be used as an http
                    // redirect response value.
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

                if (dbUser is null)
                {
                    dbUser = new Domain.Entities.AngryUser
                    {
                        UserName = username,
                        GithubAvatarUrl = avatarUrl,
                        GithubPat = pat,
                        Name = name,
                        Status = "Initialized",
                        GithubProfile = githubProfile
                    };

                    dbContext.Users.Add(dbUser);
                }
                else
                {
                    dbUser.UserName = username;
                    dbUser.GithubAvatarUrl = avatarUrl;
                    dbUser.GithubPat = pat;
                    dbUser.Name = name;
                    dbUser.GithubProfile = githubProfile;
                }

                await dbContext.SaveChangesAsync(new CancellationTokenSource().Token);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    return LocalRedirect("/");
                }
            }
            catch (AuthorizationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Page();
        }
    }
}
