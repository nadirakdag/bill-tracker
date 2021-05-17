using Application.Common.Data;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class BillTrackerDbContext : DbContext, IBillTrackerDbContext
    {
        public BillTrackerDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bill> Bills { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillTrackerDbContext).Assembly);
        }
    }
}