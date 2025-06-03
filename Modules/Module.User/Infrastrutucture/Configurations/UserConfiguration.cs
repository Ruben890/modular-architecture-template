using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.User.Domain.Entity;
namespace Module.User.Infrastrutucture.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> entity)
        {
            entity.ToTable(nameof(User));

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email)
                .IsUnique();


            entity.Property(e => e.Email)
               .IsRequired()
               .HasMaxLength(256);

            entity.Property(e => e.UserName)
                .HasMaxLength(60);

            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();
        }
    }
}