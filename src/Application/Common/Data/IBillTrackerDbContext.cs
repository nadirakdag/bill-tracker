using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Data
{
    public interface IBillTrackerDbContext
    {
        DbSet<Bill> Bills { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}