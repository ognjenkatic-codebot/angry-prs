using AngryPullRequests.Application.AngryPullRequests.Commands;
using AngryPullRequests.Domain.Entities;
using AngryPullRequests.Web.Pages;
using AutoMapper;

namespace AngryPullRequests.Web.Util.Mappings
{
    public class RepositoryWebProfile : Profile
    {
        public RepositoryWebProfile()
        {
            CreateMap<RepositoryModel.Model, UpdateRepositoryCommand>();
            CreateMap<NewRepositoryModel.Model, CreateRepositoryCommand>();

            CreateMap<RepositoryModel.Model, Repository>()
                .ForMember(
                    dst => dst.Characteristics,
                    opt =>
                        opt.MapFrom(
                            src =>
                                new RepositoryCharacteristics
                                {
                                    OldPrAgeInDays = src.OldPrAgeInDays,
                                    LargePrChangeCount = src.LargePrChangeCount,
                                    DeleteHeavyRatio = src.DeleteHeavyRatio,
                                    InactivePrAgeInDays = src.InactivePrAgeInDays,
                                    InProgressLabel = src.InProgressLabel,
                                    IssueBaseUrl = src.IssueBaseUrl,
                                    IssueRegex = src.IssueRegex,
                                    PullRequestNameCaptureRegex = src.PullRequestNameCaptureRegex,
                                    PullRequestNameRegex = src.PullRequestNameRegex,
                                    ReleaseTagRegex = src.ReleaseTagRegex,
                                    SlackAccessToken = src.SlackAccessToken,
                                    SlackApiToken = src.SlackApiToken,
                                    SlackNotificationChannel = src.SlackNotificationChannel,
                                    SmallPrChangeCount = src.SmallPrChangeCount
                                }
                        )
                )
                .ForMember(
                    dst => dst.RunSchedule,
                    opt => opt.MapFrom(src => new RunSchedule { DaysOfWeek = new int[] { 1, 2, 3, 4, 5 }, TimeOfDay = src.TimeOfDay })
                );

            CreateMap<Repository, RepositoryModel.Model>()
                .ForMember(dst => dst.TimeOfDay, opt => opt.MapFrom(src => src.RunSchedule.TimeOfDay))
                .ForMember(dst => dst.IssueRegex, opt => opt.MapFrom(src => src.Characteristics.IssueRegex))
                .ForMember(dst => dst.PullRequestNameCaptureRegex, opt => opt.MapFrom(src => src.Characteristics.PullRequestNameCaptureRegex))
                .ForMember(dst => dst.SlackAccessToken, opt => opt.MapFrom(src => src.Characteristics.SlackAccessToken))
                .ForMember(dst => dst.SlackNotificationChannel, opt => opt.MapFrom(src => src.Characteristics.SlackNotificationChannel))
                .ForMember(dst => dst.DeleteHeavyRatio, opt => opt.MapFrom(src => src.Characteristics.DeleteHeavyRatio))
                .ForMember(dst => dst.PullRequestNameRegex, opt => opt.MapFrom(src => src.Characteristics.PullRequestNameRegex))
                .ForMember(dst => dst.InactivePrAgeInDays, opt => opt.MapFrom(src => src.Characteristics.InactivePrAgeInDays))
                .ForMember(dst => dst.InProgressLabel, opt => opt.MapFrom(src => src.Characteristics.InProgressLabel))
                .ForMember(dst => dst.IssueBaseUrl, opt => opt.MapFrom(src => src.Characteristics.IssueBaseUrl))
                .ForMember(dst => dst.LargePrChangeCount, opt => opt.MapFrom(src => src.Characteristics.LargePrChangeCount))
                .ForMember(dst => dst.OldPrAgeInDays, opt => opt.MapFrom(src => src.Characteristics.OldPrAgeInDays))
                .ForMember(dst => dst.ReleaseTagRegex, opt => opt.MapFrom(src => src.Characteristics.ReleaseTagRegex))
                .ForMember(dst => dst.SlackApiToken, opt => opt.MapFrom(src => src.Characteristics.SlackApiToken))
                .ForMember(dst => dst.SmallPrChangeCount, opt => opt.MapFrom(src => src.Characteristics.SmallPrChangeCount));
        }
    }
}
