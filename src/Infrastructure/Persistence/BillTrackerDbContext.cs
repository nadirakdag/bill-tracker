using Application.Common.Data;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class BillTrackerDbContext: DbContext, IBillTrackerDbContext
    {
        public BillTrackerDbContext()
        {

        }

        public BillTrackerDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Bill> Bills { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillTrackerDbContext).Assembly);
        }
    }
}