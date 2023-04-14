using AngryPullRequests.Application.Models;
using AngryPullRequests.Application.Services;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using Autofac.Features.OwnedInstances;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public class ForgottenPullRequestsMessageFormatter : BaseSlackMessageFormatter
    {
        private readonly IPullRequestStateService pullRequestStateService;

        private readonly JiraConfiguration jiraConfiguration;
        private readonly IMetricService metricService;

        public ForgottenPullRequestsMessageFormatter(
            IPullRequestStateService pullRequestStateService,
            JiraConfiguration jiraConfiguration,
            IMetricService metricService
        )
        {
            this.pullRequestStateService = pullRequestStateService;
            this.jiraConfiguration = jiraConfiguration;
            this.metricService = metricService;
        }

        private async Task<List<Block>> GetPullRequestsMessageBlocks(User[] reviewers, PullRequest pullRequest)
        {
            var reviewersText = reviewers?.Length > 0 ? $"{string.Join(',', reviewers.Select(r => r.Login))}" : "N/A";

            var pullRequestTitle = pullRequestStateService.GetNameWithoutJiraTicket(pullRequest) ?? pullRequest.Title;

            var authorExperience = await metricService.GetAuthorExperience("vodafone-frinx-admin", "Codaxy", pullRequest.User.Login);

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

            var authorExp = pullRequestStateService.GetUserExperienceLabels(authorExperience);

            var elements = new List<IContextElement>
            {
                CreateMd($"Dana star: *{(DateTimeOffset.Now - pullRequest.CreatedAt).Days}*"),
                CreateMd($"Promjene: *{pullRequest.ChangedFiles} CF / {pullRequest.Additions} A / {pullRequest.Deletions} D*"),
                CreateMd($"Autor: *{pullRequest.User.Login}* **{authorExp[pullRequest.User.Login].ToString()}**"),
                CreateMd($"Pregleda: *{reviewersText}*"),
                CreateMd($"Base: *{pullRequest.BaseRef}*"),
                CreateMd($"Head: *{pullRequest.HeadRef}*")
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
            var users = string.Join(',', pullRequestNotificationGroups.Select(p => p.PullRequest.User.Login).ToList());

            var blocks = new List<Block> { new HeaderBlock { Text = CreatePe($"Pregled Pull Requestova") } };

            foreach (var group in pullRequestNotificationGroups)
            {
                blocks.AddRange(await GetPullRequestsMessageBlocks(group.Reviewers, group.PullRequest));
            }

            return blocks;
        }
    }
}
