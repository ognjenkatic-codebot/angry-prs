using AngryPullRequests.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using AngryPullRequests.Application.Services;

namespace AngryPullRequests.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static void AddInfrastrucutreServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database") ?? throw new Exception("Connection string not found.");
            serviceCollection.AddDbContext<AngryPullRequestsContext>(
                (sp, options) =>
                {
                    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
                }
            );

            serviceCollection.AddScoped<IAngryPullRequestsContext>(p => p.GetRequiredService<AngryPullRequestsContext>());
        }
    }
}
