using AngryPullRequests.Application.Models;
using AngryPullRequests.Application.Services;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public class ForgottenPullRequestsMessageFormatter : BaseSlackMessageFormatter
    {
        private readonly IPullRequestStateService pullRequestStateService;

        private static int previousExecutionDoy = 0;
        private static string currentMood = null;
        private static string previousMood = null;

        private readonly JiraConfiguration jiraConfiguration;
        private readonly ICompletionService completionService;
        private static readonly Dictionary<string, string> commentStyles =
            new()
            {
                { "bespomoćno", ":confounded:" },
                { "dirnuto", ":sob:" },
                { "inspirirano", ":sparkles:" },
                { "iznenađeno", ":open_mouth:" },
                { "kreativano", ":art:" },
                { "ljutito", ":angry:" },
                { "neodlučano", ":confused:" },
                { "nervozano", ":sweat_smile:" },
                { "nesigurano", ":worried:" },
                { "nezainteresovano", ":expres}sionless:" },
                { "oduševljeno", ":heart_eyes:" },
                { "optimisticno", ":smiley:" },
                { "ponosano", ":smiley:" },
                { "sretno", ":smile:" },
                { "ugnjetavano", ":frowning:" },
                { "uhvaćen u čudu", ":astonished:" },
                { "umorno", ":sleepy:" },
                { "uvjereno", ":sunglasses:" },
                { "uvrijeđeno", ":angry:" },
                { "uzbudjeno", ":grinning:" },
                { "uzdrmano", ":worried:" },
                { "uzdržano", ":neutral_face:" },
                { "veselo", ":smile:" },
                { "zabrinuto", ":frowning:" },
                { "zacudjeno", ":open_mouth:" },
                { "zadovoljno", ":slight_smile:" },
                { "zapanjeno", ":astonished:" },
                { "zbunjeno", ":confused:" },
                { "šokirano", ":scream:" },
                { "pijano", ":beer:" }
            };
        private static readonly Random random = new();

        public ForgottenPullRequestsMessageFormatter(
            IPullRequestStateService pullRequestStateService,
            JiraConfiguration jiraConfiguration,
            ICompletionService completionService
        )
        {
            this.pullRequestStateService = pullRequestStateService;
            this.jiraConfiguration = jiraConfiguration;
            this.completionService = completionService;

            currentMood ??= GetMood();
            previousMood ??= GetMood();
        }

        private async Task<List<Block>> GetPullRequestsMessageBlocks(User[] reviewers, PullRequest pullRequest)
        {
            var reviewersText = reviewers?.Length > 0 ? $"{string.Join(',', reviewers.Select(r => r.Login))}" : "N/A";

            var pullRequestTitle = pullRequestStateService.GetNameWithoutJiraTicket(pullRequest) ?? pullRequest.Title;

            var blocks = new List<Block>
            {
                new SectionBlock { Text = CreateMd($"Pull request <{pullRequest.HtmlUrl}|{pullRequestTitle}> jos uvijek nije odobren"), }
            };

            var fields = new List<TextObject>();

            var characteristics = new List<string>();

            if (!pullRequestStateService.FollowsNamingConventions(pullRequest))
            {
                fields.Add(CreatePe($":hankey: Lose ime"));
                characteristics.Add("lose ime");
            }

            if (!pullRequestStateService.HasReviewer(pullRequest))
            {
                fields.Add(CreatePe($":broken_heart: Potreban reviewer"));
                characteristics.Add("nije dodjeljen revieweru");
            }

            if (pullRequestStateService.IsHuge(pullRequest))
            {
                fields.Add(CreatePe($":oncoming_bus: Ogroman"));
                characteristics.Add("ogroman");
            }

            if (pullRequestStateService.IsSmall(pullRequest))
            {
                fields.Add(CreatePe($":hedgehog: Malen"));
                characteristics.Add("malen");
            }

            if (pullRequestStateService.IsOld(pullRequest))
            {
                fields.Add(CreatePe($":skull: Star"));
                characteristics.Add("star");
            }

            if (pullRequestStateService.IsInactive(pullRequest))
            {
                fields.Add(CreatePe($":sloth: Neaktivan"));
                characteristics.Add("neaktivan");
            }

            if (pullRequestStateService.IsDeleteHeavy(pullRequest))
            {
                fields.Add(CreatePe($":scissors: Uglavnom brisanje"));
                characteristics.Add("uglavnom brisanje");
            }

            if (pullRequestStateService.IsInProgress(pullRequest))
            {
                fields.Add(CreatePe($":construction_worker: Nije gotov"));
                characteristics.Add("nije gotov");
            }

            if (!pullRequestStateService.HasReleaseTag(pullRequest))
            {
                fields.Add(CreatePe($":chicken: Nema release tag"));
                characteristics.Add("nema release tag");
            }

            if (pullRequestStateService.DoesLikelyHaveConflicts(pullRequest))
            {
                fields.Add(CreatePe($":pouting_cat: Ima konflikte"));
                characteristics.Add("ima konflikte");
            }

            if (fields.Count > 0)
            {
                blocks.Add(new SectionBlock { Fields = fields });
            }

            characteristics.Add($"autor je {pullRequest.User.Login}");
            characteristics.Add($"za review su zaduzeni {reviewersText}");

            var props = characteristics.Count > 0 ? string.Join(',', characteristics) : string.Empty;

            var promptResponse = await completionService.GetCompletion(
                $"Ti si slack bot koji obavjestava developere o pull requestovima koji dugo stoje. "
                    + $"Komentar treba navesti developera da pogleda svoj pull request. \n"
                    + $"Dobices neke karakteristike pull requesta koje trebas iskoristiti u svom komentaru tako sto ces ih preuvelicati i istaci, ali dati i predlog kako da buduci bude bolji i pohvaliti ono sto je vec dobro. \n"
                    + $"Karakteristike pull requesta su: {props}, ti se osjecas {currentMood}, tvoj komentar je:"
            );

            promptResponse = promptResponse.Replace("\"", "");

            var elements = new List<IContextElement>
            {
                CreateMd($"Dana star: *{(DateTimeOffset.Now - pullRequest.CreatedAt).Days}*"),
                CreateMd($"Promjene: *{pullRequest.ChangedFiles} CF / {pullRequest.Additions} A / {pullRequest.Deletions} D*"),
                CreateMd($"Autor: *{pullRequest.User.Login}*"),
                CreateMd($"Pregleda: *{reviewersText}*"),
                CreateMd($"Base: *{pullRequest.BaseRef}*"),
                CreateMd($"Head: *{pullRequest.HeadRef}*"),
                CreateMd($"Komentar: *{promptResponse.Trim()}*")
            };

            var jiraTicketName = pullRequestStateService.GetJiraTicket(pullRequest);

            if (!string.IsNullOrEmpty(jiraTicketName))
            {
                elements.Add(CreateMd($"Jira: *<{jiraConfiguration.IssueBaseUrl}{jiraTicketName}|{jiraTicketName}>*"));
            }

            blocks.Add(new ContextBlock { Elements = elements });

            blocks.Add(new DividerBlock());

            return blocks;
        }

        public override async Task<List<Block>> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups)
        {
            if (DateTime.Now.DayOfYear != previousExecutionDoy)
            {
                currentMood = GetMood();
                previousExecutionDoy = DateTime.Now.DayOfYear;
            }

            var users = string.Join(',', pullRequestNotificationGroups.Select(p => p.PullRequest.User.Login).ToList());

            var promptResponse = await completionService.GetCompletion(
                $"Ti si slack bot koji obavjestava developere o pull requestovima koji dugo stoje i svaki dan imas drugi raspolozenje. Ti nisi developer, vec im samo pomazes, i zelis to da radis sto bolje."
                    + $"Kao jedan od razloga promjene tvog raspolozenja navedi neke od {users} kojima pripadaju pull requestovi. "
                    + $"Tvoji odgovori pocinju sa angrypr, na primjer: \n"
                    + "angrypr: danas je moje raspolozenje nepromjenjeno, i dalje se osjecam tuzno, imam osjecaj da nista sto radim ovdje nema efekta na vase pull requestove"
                    + $"Danas se osjecas {currentMood}, objasni zasto se osjecas tako na nacin u skladu sa tvojim trenutnim raspolozenjem i spomeni tvoje proslo i sadasnje raspolozenje. Juce si se osjecao osjecao {previousMood}. angrypr:"
            );

            var blocks = new List<Block>
            {
                new HeaderBlock { Text = CreatePe($"Danas se osjecam {currentMood} {commentStyles[currentMood]}") },
                new SectionBlock { Text = CreateMd($"{promptResponse}") },
                new DividerBlock()
            };

            foreach (var group in pullRequestNotificationGroups)
            {
                blocks.AddRange(await GetPullRequestsMessageBlocks(group.Reviewers, group.PullRequest));
            }

            return blocks;
        }

        private string GetMood()
        {
            return commentStyles.Keys.ElementAt(random.Next(commentStyles.Keys.Count));
        }
    }
}
