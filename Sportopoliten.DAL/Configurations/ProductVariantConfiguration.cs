using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Configurations
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Color)
                .HasMaxLength(50);

            builder.Property(x => x.Size)
                .HasMaxLength(20);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Stock)
                .IsRequired()
                .HasDefaultValue(0);
                

            builder
                .HasMany(x => x.Images)
                .WithOne(x => x.ProductVariant)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
