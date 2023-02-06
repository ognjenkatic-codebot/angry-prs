using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using SlackNet.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryPullRequests.Application.Slack.Formatters
{
    public class ForgottenPullRequestsMessageFormatter : BaseSlackMessageFormatter
    {
        private readonly IPullRequestStateService pullRequestStateService;

        public ForgottenPullRequestsMessageFormatter(IPullRequestStateService pullRequestStateService)
        {
            this.pullRequestStateService = pullRequestStateService;
        }

        private List<Block> GetPullRequestsMessageBlocks(User[] reviewers, PullRequest pullRequest)
        {
            var reviewersText = reviewers?.Length > 0 ? $"{string.Join(',', reviewers.Select(r => r.Login))}" : "N/A";

            var blocks = new List<Block>
            {
                new SectionBlock { Text = CreateMd($"Pull request <{pullRequest.HtmlUrl}|{pullRequest.Title}> jos uvijek nije pregledan"), }
            };

            var fields = new List<TextObject>();

            if (!pullRequestStateService.FollowsNamingConventions(pullRequest))
            {
                fields.Add(CreatePe($":hankey: Lose ime"));
            }

            if (!pullRequestStateService.HasReviewer(pullRequest))
            {
                fields.Add(CreatePe($":broken_heart: Potreban reviewer"));
            }

            if (pullRequestStateService.IsHuge(pullRequest))
            {
                fields.Add(CreatePe($":oncoming_bus: Ogroman"));
            }

            if (pullRequestStateService.IsSmall(pullRequest))
            {
                fields.Add(CreatePe($":hedgehog: Malen"));
            }

            if (pullRequestStateService.IsOld(pullRequest))
            {
                fields.Add(CreatePe($":skull: Star"));
            }

            if (pullRequestStateService.IsInactive(pullRequest))
            {
                fields.Add(CreatePe($":sloth: Neaktivan"));
            }

            if (pullRequestStateService.IsDeleteHeavy(pullRequest))
            {
                fields.Add(CreatePe($":scissors: Uglavnom brisanje"));
            }

            if (pullRequestStateService.IsInProgress(pullRequest))
            {
                fields.Add(CreatePe($":construction_worker: Nije gotov"));
            }

            if (!pullRequestStateService.HasReleaseTag(pullRequest))
            {
                fields.Add(CreatePe($":chicken: Nema release tag"));
            }

            if (pullRequestStateService.DoesLikelyHaveConflicts(pullRequest))
            {
                fields.Add(CreatePe($":pouting_cat: Ima konflikte"));
            }

            if (fields.Count > 0)
            {
                blocks.Add(new SectionBlock { Fields = fields });
            }

            blocks.Add(
                new ContextBlock
                {
                    Elements =
                    {
                        CreateMd($"Dana star: *{(DateTimeOffset.Now - pullRequest.CreatedAt).Days}*"),
                        CreateMd($"Promjene: *{pullRequest.ChangedFiles} CF/ {pullRequest.Additions} A/ {pullRequest.Deletions} D*"),
                        CreateMd($"Autor: *{pullRequest.User.Login}*"),
                        CreateMd($"Pregleda: *{reviewersText}*")
                    }
                }
            );

            blocks.Add(new DividerBlock());

            return blocks;
        }

        public override List<Block> GetBlocks(PullRequestNotificationGroup[] pullRequestNotificationGroups)
        {
            var blocks = new List<Block> { new HeaderBlock { Text = CreatePe("Zaboravljeni pull requestovi :sneezing_face:") } };

            foreach (var group in pullRequestNotificationGroups)
            {
                blocks.AddRange(GetPullRequestsMessageBlocks(group.Reviewers, group.PullRequest));
            }

            return blocks;
        }
    }
}
