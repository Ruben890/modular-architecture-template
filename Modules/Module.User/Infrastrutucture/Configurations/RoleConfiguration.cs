﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.User.Domain.Entity;

namespace Module.User.Infrastrutucture.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.ToTable(nameof(Role));

            builder.HasIndex(r => r.Name).IsUnique(); // Name es la PK
            builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        }
    }
}
