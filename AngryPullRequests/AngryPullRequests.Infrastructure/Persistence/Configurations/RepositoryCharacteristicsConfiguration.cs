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
        }
    }
}
