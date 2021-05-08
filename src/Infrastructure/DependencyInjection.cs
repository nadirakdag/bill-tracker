using Application.Common.Data;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BillTrackerDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("BillTrackerDbConnection")));

            services.AddScoped<IBillTrackerDbContext>(provider => provider.GetService<BillTrackerDbContext>());

            return services;
        }
    }
}