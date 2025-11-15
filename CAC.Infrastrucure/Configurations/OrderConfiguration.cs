using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAC.Domain.Entities;

namespace CAC.Infrastrucure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.OrderDate)
            .IsRequired();
        
        builder.Property(e => e.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(e => e.Status)
            .IsRequired();

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.OrderItems)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

