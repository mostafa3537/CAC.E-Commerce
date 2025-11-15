using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAC.Domain.Entities;

namespace CAC.Infrastrucure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(e => e.Description)
            .HasMaxLength(1000);
        
        builder.Property(e => e.CreatedDate)
            .IsRequired();
        
        builder.Property(e => e.UpdatedDate)
            .IsRequired();
    }
}

