using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                .HasMaxLength(150);

            builder.Property(x => x.Phone)
                .HasMaxLength(20);

            builder.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Login)
                .IsUnique();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.Property(x => x.Salt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder
                .HasOne(x => x.Cart)
                .WithOne(x => x.User)
                .HasForeignKey<Cart>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
