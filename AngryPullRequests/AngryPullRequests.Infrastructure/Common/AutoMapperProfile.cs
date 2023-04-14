using AngryPullRequests.Domain.Models;
using AutoMapper;

namespace AngryPullRequests.Infrastructure.Common
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Octokit.PullRequest, PullRequest>()
                .ForMember(pr => pr.BaseRef, e => e.MapFrom(pr => pr.Base.Ref))
                .ForMember(pr => pr.HeadRef, e => e.MapFrom(pr => pr.Head.Ref));

            CreateMap<Octokit.Label, Label>();
            CreateMap<Octokit.Account, Account>();
            CreateMap<Octokit.User, User>();
            CreateMap<Octokit.PullRequestReview, PullRequestReview>();
        }
    }
}
