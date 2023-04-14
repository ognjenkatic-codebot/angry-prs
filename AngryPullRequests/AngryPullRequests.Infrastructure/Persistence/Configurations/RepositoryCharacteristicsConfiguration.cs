using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AngryPullRequests.Infrastructure.Persistence.Configurations
{
    public class RepositoryCharacteristicsConfiguration : IEntityTypeConfiguration<RepositoryCharacteristics>
    {
        public void Configure(EntityTypeBuilder<RepositoryCharacteristics> builder)
        {
            builder.HasKey(r => r.RepositoryId);
            builder.HasOne(r => r.Repository).WithOne(r => r.Characteristics).HasForeignKey<RepositoryCharacteristics>(r => r.RepositoryId);

            builder.Property(r => r.InProgressLabel).IsRequired();
            builder.Property(r => r.SmallPrChangeCount).IsRequired();
            builder.Property(r => r.LargePrChangeCount).IsRequired();
            builder.Property(r => r.InactivePrAgeInDays).IsRequired();
            builder.Property(r => r.OldPrAgeInDays).IsRequired();
            builder.Property(r => r.DeleteHeavyRatio).IsRequired();
            builder.Property(r => r.PullRequestNameCaptureRegex).IsRequired();
            builder.Property(r => r.PullRequestNameRegex).IsRequired();
            builder.Property(r => r.ReleaseTagRegex).IsRequired();
            builder.Property(r => r.SlackAccessToken).IsRequired();
            builder.Property(r => r.SlackApiToken).IsRequired();
            builder.Property(r => r.SlackNotificationChannel).IsRequired();
            builder.Property(r => r.IssueBaseUrl).IsRequired();
            builder.Property(r => r.IssueRegex).IsRequired();
        }
    }
}
