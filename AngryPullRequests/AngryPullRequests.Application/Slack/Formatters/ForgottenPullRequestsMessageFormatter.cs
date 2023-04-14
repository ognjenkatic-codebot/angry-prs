using AngryPullRequests.Domain.Models;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AngryPullRequests.Application.Persistence;
using AngryPullRequests.Application.AngryPullRequests.Interfaces;
using AngryPullRequests.Application.AngryPullRequests;
using AngryPullRequests.Application.AngryPullRequests.Models;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public class ForgottenPullRequestsMessageFormatter : BaseSlackMessageFormatter
    {
        private readonly IMetricService metricService;
        private readonly IAngryPullRequestsContext dbContext;

        public ForgottenPullRequestsMessageFormatter(IMetricService metricService, IAngryPullRequestsContext dbContext)
        {
            this.metricService = metricService;
            this.dbContext = dbContext;
        }

        private async Task<List<Block>> GetPullRequestsMessageBlocks(
            User[] reviewers,
            PullRequest pullRequest,
            string repositoryName,
            string repositoryOwner
        )
        {
            var dbRepository = await dbContext.Repositories
                .Include(r => r.Characteristics)
                .FirstAsync(r => r.Name == repositoryName && r.Owner == repositoryOwner);

            var issueBaseUrl = dbRepository.Characteristics.IssueBaseUrl;

            var pullRequestStateService = new PullRequestStateService(dbRepository.Characteristics);

            var reviewersText = reviewers?.Length > 0 ? $"{string.Join(',', reviewers.Select(r => r.Login))}" : "N/A";

            var pullRequestTitle = pullRequestStateService.GetNameWithoutJiraTicket(pullRequest) ?? pullRequest.Title;

            //var authorExperience = await metricService.GetAuthorExperience("vodafone-frinx-admin", "Codaxy", pullRequest.User.Login);

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

            //var authorExp = pullRequestStateService.GetUserExperienceLabels(authorExperience);

            var elements = new List<IContextElement>
            {
                CreateMd($"Dana star: *{(DateTimeOffset.Now - pullRequest.CreatedAt).Days}*"),
                CreateMd($"Promjene: *{pullRequest.ChangedFiles} CF / {pullRequest.Additions} A / {pullRequest.Deletions} D*"),
                //CreateMd($"Autor: *{pullRequest.User.Login}* **{authorExp[pullRequest.User.Login].ToString()}**"),
                CreateMd($"Pregleda: *{reviewersText}*"),
                CreateMd($"Base: *{pullRequest.BaseRef}*"),
                CreateMd($"Head: *{pullRequest.HeadRef}*")
            };

            var jiraTicketName = pullRequestStateService.GetJiraTicket(pullRequest);

            if (!string.IsNullOrEmpty(jiraTicketName))
            {
                elements.Add(CreateMd($"Jira: *<{issueBaseUrl}{jiraTicketName}|{jiraTicketName}>*"));
            }

            blocks.Add(new ContextBlock { Elements = elements });

            blocks.Add(new DividerBlock());

            return blocks;
        }

        public override async Task<List<Block>> GetBlocks(
            PullRequestNotificationGroup[] pullRequestNotificationGroups,
            string repositoryName,
            string repositoryOwner
        )
        {
            var users = string.Join(',', pullRequestNotificationGroups.Select(p => p.PullRequest.User.Login).ToList());

            var blocks = new List<Block> { new HeaderBlock { Text = CreatePe($"Pregled Pull Requestova") } };

            var tasks = pullRequestNotificationGroups.Select(
                ng => GetPullRequestsMessageBlocks(ng.Reviewers, ng.PullRequest, repositoryName, repositoryOwner)
            );

            var taskResponses = await Task.WhenAll(tasks);

            blocks.AddRange(taskResponses.SelectMany(b => b));

            return blocks;
        }
    }
}
