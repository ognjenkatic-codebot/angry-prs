﻿using AngryPullRequests.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Application.Github
{
    public interface IPullRequestServiceFactory
    {
        Task<IPullRequestService> Create(string repositoryName, string repositoryOwner);
    }

    public interface IPullRequestService
    {
        Task<PullRequest[]> GetPullRequests(string owner, string repository, bool getAll, int pageCount, int pageSize, int startPage);
        Task<User[]> GetRequestedReviewersUsers(string owner, string repository, int pullRequestNumber);
        Task<PullRequestReview[]> GetPullRequsetReviews(string owner, string repository, int pullRequestNumber);
        Task<PullRequest> GetPullRequestDetails(string owner, string repository, int pullRequestNumber);
    }
}
