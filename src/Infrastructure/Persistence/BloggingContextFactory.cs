using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class BloggingContextFactory : IDesignTimeDbContextFactory<BillTrackerDbContext>
    {
        public BillTrackerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BillTrackerDbContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=mysecretpassword;Host=localhost;Port=5432;Database=BillTracker;Pooling=true;");

            return new BillTrackerDbContext(optionsBuilder.Options);
        }
    }
}