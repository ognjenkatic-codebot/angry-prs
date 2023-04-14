using AngryPullRequests.Application.Models;
using AngryPullRequests.Application.Services;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.OpenAi
{
    public class OpenAiCompletionService : ICompletionService
    {
        private OpenAIAPI Api { get; set; }

        public OpenAiCompletionService(OpenAiConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Token))
            {
                throw new ArgumentNullException("Token must be specified");
            }

            Api = new OpenAIAPI(configuration.Token);
        }

        public async Task<string> GetCompletion(string prompt)
        {
            string text;

            try
            {
                var result = await Api.Completions.CreateCompletionAsync(
                    new CompletionRequest(prompt, model: Model.DavinciText, temperature: 0.7, max_tokens: 2000)
                );

                text = result.Completions[0].Text.Replace("\n", "").Trim();
            }
            catch (Exception)
            {
                text = "Service unavailable";
            }

            return text;
        }
    }
}
