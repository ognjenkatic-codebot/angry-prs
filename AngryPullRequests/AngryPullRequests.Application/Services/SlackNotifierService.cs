using AngryPullRequests.Application.Models;
using AngryPullRequests.Domain.Models;
using AngryPullRequests.Domain.Services;
using AngryPullRequests.Infrastructure.Models;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Services
{
    public class SlackNotifierService : IUserNotifierService
    {
        private readonly ISlackApiClient slack;
        private readonly IPullRequestStateService pullRequestStateService;
        private readonly SlackConfiguration slackConfiguration;

        public SlackNotifierService(ISlackApiClient slack, IPullRequestStateService pullRequestStateService, SlackConfiguration slackConfiguration)
        {
            this.slack = slack;
            this.pullRequestStateService = pullRequestStateService;
            this.slackConfiguration = slackConfiguration;
        }

        public async Task Notify(PullRequestNotificationGroup[] pullRequestNotificationGroups)
        {
            var blocks = new List<Block>
            {
                new HeaderBlock
                {
                    Text = new PlainText { Text = "Zaboravljeni pull requestovi :sneezing_face:", Emoji = true }
                }
            };

            foreach (var group in pullRequestNotificationGroups)
            {
                blocks.AddRange(GetPullRequestsMessageBlocks(group.Reviewers, group.PullRequest));
            }

            var loadsBlocks = GetLoadsMessageBlock(pullRequestNotificationGroups);

            await slack.Chat.PostMessage(new Message { Blocks = blocks, Channel = slackConfiguration.NotificationsChannel }).ConfigureAwait(false);
            await slack.Chat
                .PostMessage(new Message { Blocks = loadsBlocks, Channel = slackConfiguration.NotificationsChannel })
                .ConfigureAwait(false);
        }

        private List<Block> GetLoadsMessageBlock(PullRequestNotificationGroup[] pullRequestNotificationGroups)
        {
            var prsByUser = pullRequestNotificationGroups
                .SelectMany(prg => prg.Reviewers)
                .GroupBy(r => r.Login)
                .Select(group => new { Login = group.Key, Count = group.Count() });

            var userBlocks = new List<Block>
            {
                new HeaderBlock
                {
                    Text = new PlainText() { Text = $"Opterecenje po developeru :face_with_peeking_eye:", Emoji = true }
                }
            };

            foreach (var user in prsByUser)
            {
                var emoji = user.Count switch
                {
                    1 => ":slightly_smiling_face:",
                    <= 3 => ":neutral_face:",
                    <= 5 => ":nauseated_face:",
                    > 5 => ":face_vomiting:"
                };

                userBlocks.Add(
                    new ContextBlock
                    {
                        Elements =
                        {
                            new PlainText { Text = $"{user.Login}: {emoji} {user.Count}", Emoji = true }
                        }
                    }
                );
            }
            return userBlocks;
        }

        private List<Block> GetPullRequestsMessageBlocks(Domain.Models.User[] reviewers, PullRequest pullRequest)
        {
            var reviewersText = reviewers?.Length > 0 ? $"{string.Join(',', reviewers.Select(r => r.Login))}" : "N/A";

            var blocks = new List<Block>
            {
                new SectionBlock
                {
                    Text = new Markdown() { Text = $"Pull request <{pullRequest.HtmlUrl}|{pullRequest.Title}> jos uvijek nije pregledan" },
                }
            };

            var fields = new List<TextObject>();

            if (!pullRequestStateService.FollowsNamingConventions(pullRequest))
            {
                fields.Add(new PlainText { Text = $":hankey: Lose ime", Emoji = true });
            }

            if (!pullRequestStateService.HasReviewer(pullRequest))
            {
                fields.Add(new PlainText { Text = $":broken_heart: Potreban reviewer", Emoji = true });
            }

            if (pullRequestStateService.IsHuge(pullRequest))
            {
                fields.Add(new PlainText { Text = $":oncoming_bus: Ogroman", Emoji = true });
            }

            if (pullRequestStateService.IsSmall(pullRequest))
            {
                fields.Add(new PlainText { Text = $":hedgehog: Malen", Emoji = true });
            }

            if (pullRequestStateService.IsOld(pullRequest))
            {
                fields.Add(new PlainText { Text = $":skull: Star", Emoji = true });
            }

            if (pullRequestStateService.IsInactive(pullRequest))
            {
                fields.Add(new PlainText { Text = $":sloth: Neaktivan", Emoji = true });
            }

            if (pullRequestStateService.IsDeleteHeavy(pullRequest))
            {
                fields.Add(new PlainText { Text = $":scissors: Uglavnom brisanje", Emoji = true });
            }

            if (pullRequestStateService.IsInProgress(pullRequest))
            {
                fields.Add(new PlainText { Text = $":construction_worker: Nije gotov", Emoji = true });
            }

            if (!pullRequestStateService.HasReleaseTag(pullRequest))
            {
                fields.Add(new PlainText { Text = $":chicken: Nema release tag", Emoji = true });
            }

            if (pullRequestStateService.DoesLikelyHaveConflicts(pullRequest))
            {
                fields.Add(new PlainText { Text = $":pouting_cat: Ima konflikte", Emoji = true });
            }

            if (fields.Count > 0)
            {
                blocks.Add(new SectionBlock { Fields = fields });
            }

            var age = DateTimeOffset.Now - pullRequest.CreatedAt;

            blocks.Add(
                new ContextBlock
                {
                    Elements =
                    {
                        new Markdown { Text = $"Dana star: *{age.Days}*" },
                        new Markdown { Text = $"Promjene: *{pullRequest.ChangedFiles} CF/ {pullRequest.Additions} A/ {pullRequest.Deletions} D*" },
                        new Markdown { Text = $"Autor: *{pullRequest.User.Login}*" },
                        new Markdown { Text = $"Pregleda: *{reviewersText}*" }
                    }
                }
            );

            blocks.Add(new DividerBlock());
            return blocks;
        }
    }
}
