using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAC.Domain.Entities;

namespace CAC.Infrastrucure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Quantity)
            .IsRequired();
        
        builder.Property(e => e.PriceAtOrder)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

