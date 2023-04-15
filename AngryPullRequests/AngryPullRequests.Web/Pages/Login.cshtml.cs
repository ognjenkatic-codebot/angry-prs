using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using System.Security.Claims;

namespace AngryPullRequests.Web.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect("/");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string pat, string returnUrl = null)
        {
            try
            {
                if (HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    return LocalRedirect("/");
                }

                var gitHubClient = new GitHubClient(new ProductHeaderValue("AngryPullRequests")) { Credentials = new Credentials(pat) };

                var pt = await gitHubClient.User.Current();

                var claims = new List<Claim>
                {
                    new Claim("AvatarUrl", pt.AvatarUrl ?? ""),
                    new Claim("Username", pt.Login),
                    new Claim("GithubProfile", pt.Url),
                    new Claim(ClaimTypes.Name, pt.Name ?? "")
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
