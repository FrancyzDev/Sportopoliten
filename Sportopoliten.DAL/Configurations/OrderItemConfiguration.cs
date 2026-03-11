using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.PriceAtPurchase)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Subtotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Count)
                .IsRequired();

            builder
                .HasOne(x => x.ProductVariant)
                .WithMany()
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
