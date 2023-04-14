using AngryPullRequests.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AngryPullRequests.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Func<IAngryPullRequestsService> angryPullRequestServiceFactory;

        public IndexModel(ILogger<IndexModel> logger, Func<IAngryPullRequestsService> angryPullRequestServiceFactory)
        {
            _logger = logger;
            this.angryPullRequestServiceFactory = angryPullRequestServiceFactory;
        }

        public async void OnGet()
        {
            await angryPullRequestServiceFactory().CheckOutPullRequests();
        }
    }
}
