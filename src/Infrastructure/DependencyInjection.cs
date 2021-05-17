using Application.Common.Data;
using Application.Common.Models;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, DatabaseConnection databaseConnection)
        {
            services.AddDbContext<BillTrackerDbContext>(options =>
                options.UseNpgsql(databaseConnection.ToString()));

            services.AddScoped<IBillTrackerDbContext>(provider => provider.GetService<BillTrackerDbContext>());

            return services;
        }

        public static IApplicationBuilder UseInfrastucture(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<BillTrackerDbContext>()?.Database.Migrate();
            }

            return app;
        }
    }
}