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
            throw new NotImplementedException();
        }
    }
}
