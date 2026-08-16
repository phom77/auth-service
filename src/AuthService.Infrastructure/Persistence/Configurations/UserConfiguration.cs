using AuthService.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Persistence.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(
        EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.Id)
                .HasName("pk_users");

            builder.Property(user => user.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(user => user.NormalizedEmail)
                .HasColumnName("normalized_email")
                .HasMaxLength(320)
                .IsRequired();

            builder.HasIndex(user => user.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("ux_users_normalized_email");

            builder.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(user => user.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(user => user.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            builder.Property(user => user.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        }
    }
}
