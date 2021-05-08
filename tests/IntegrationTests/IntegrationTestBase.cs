using System;
using Bogus;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly BillTrackerDbContext _billTrackerDbContext;

        public IntegrationTestBase()
        {
            var options = new DbContextOptionsBuilder<BillTrackerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _billTrackerDbContext = new BillTrackerDbContext(options);
            _billTrackerDbContext.Database.EnsureCreated();

            Seed(_billTrackerDbContext);
        }

        private void Seed(BillTrackerDbContext billTrackerDbContext)
        {
            var bills = new Faker<Bill>()
                .RuleFor(p => p.Amount, f => f.Finance.Amount())
                .RuleFor(p => p.Description, f => f.Random.String())
                .RuleFor(p => p.Status, f => f.PickRandom<BillStatus>())
                .RuleFor(p => p.BillDate, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2021, 12, 31)))
                .RuleFor(p => p.CreatedDate, f => f.Date.Recent())
                .RuleFor(p => p.PaidDate, f => f.Date.Soon())
                .Generate(10);

            billTrackerDbContext.Bills.AddRange(bills);
            billTrackerDbContext.SaveChanges();
        }

        public void Dispose()
        {
            _billTrackerDbContext.Database.EnsureDeleted();
            _billTrackerDbContext.Dispose();
        }
    }
}