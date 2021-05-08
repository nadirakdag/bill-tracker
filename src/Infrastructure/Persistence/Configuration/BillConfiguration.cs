using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("uuid").HasDefaultValueSql("uuid_generate_v4()");
            builder.Property(x => x.Description).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Amount).IsRequired().HasColumnType("money");
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.BillDate).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.PaidDate);
        }
    }
}