using AngryPullRequests.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AngryPullRequests.Infrastructure.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Octokit.PullRequest, PullRequest>();
            CreateMap<Octokit.Label, Label>();
            CreateMap<Octokit.Account, Account>();
            CreateMap<Octokit.User, User>();
            CreateMap<Octokit.PullRequestReview, PullRequestReview>();
        }
    }
}
