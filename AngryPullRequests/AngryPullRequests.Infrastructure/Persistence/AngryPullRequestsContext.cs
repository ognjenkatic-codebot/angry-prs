using AngryPullRequests.Application.Services;
using AngryPullRequests.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AngryPullRequests.Infrastructure.Persistence
{
    public class AngryPullRequestsContext : DbContext, IAngryPullRequestsContext
    {
        public DbSet<AngryUser> Users { get; set; }
        public DbSet<Repository> Repositories { get; set; }

        public AngryPullRequestsContext(DbContextOptions<AngryPullRequestsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
