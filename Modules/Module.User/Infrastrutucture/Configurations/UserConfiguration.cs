using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserEntity = Module.User.Domain.Entity.User;
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

            entity.HasIndex(e => e.UserName)
                .IsUnique();

            entity.Property(e => e.Email)
               .IsRequired()
               .HasMaxLength(256);

            entity.Property(e => e.UserName)
                .IsRequired()
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