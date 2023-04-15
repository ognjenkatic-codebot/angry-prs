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
    public class AngryUserConfiguration : IEntityTypeConfiguration<AngryUser>
    {
        public void Configure(EntityTypeBuilder<AngryUser> builder)
        {
            builder.HasMany(u => u.Repositories).WithOne(r => r.AngryUser).HasForeignKey(r => r.AngryUserId);
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");

            builder.Property(u => u.UserName).IsRequired();
            builder.Property(u => u.Name).IsRequired();
        }
    }
}
