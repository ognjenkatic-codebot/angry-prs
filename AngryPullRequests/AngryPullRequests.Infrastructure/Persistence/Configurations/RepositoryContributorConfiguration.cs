using AngryPullRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryPullRequests.Infrastructure.Persistence.Configurations
{
    public class RepositoryContributorConfiguration : IEntityTypeConfiguration<RepositoryContributor>
    {
        public void Configure(EntityTypeBuilder<RepositoryContributor> builder)
        {
            builder.HasKey(e => new { e.RepositoryId, e.ContributorId });

            builder.HasOne(rc => rc.Contributor).WithMany(c => c.Contributions).HasForeignKey(c => c.ContributorId);
            builder.HasOne(rc => rc.Repository).WithMany(r => r.Contributions).HasForeignKey(c => c.Repository);
        }
    }
}
