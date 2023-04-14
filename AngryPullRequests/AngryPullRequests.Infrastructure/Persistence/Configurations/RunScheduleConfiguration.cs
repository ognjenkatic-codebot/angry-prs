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
    public class RunScheduleConfiguration : IEntityTypeConfiguration<RunSchedule>
    {
        public void Configure(EntityTypeBuilder<RunSchedule> builder)
        {
            builder.HasOne(s => s.Repository).WithOne(r => r.RunSchedule).HasForeignKey<RunSchedule>(s => s.RepositoryId);
            builder.HasKey(s => s.RepositoryId);
        }
    }
}
