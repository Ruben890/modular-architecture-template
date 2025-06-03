using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Module.User.Domain.Entity
{

    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }


        [Required]
        public string? Phone { get; set; }

        [AllowNull]
        public string? UserName { get; set; }

        [AllowNull]
        public string? LastName { get; set; } = null!;
        [Required]
        public string? Email { get; set; }

        [AllowNull]
        public string? Password { get; set; }

        [AllowNull]
        public string? Dni { get; set; } = null;

        [AllowNull]
        public string? RefreshToken { get; set; } = null;

        [AllowNull]
        public DateOnly? RefreshTokenExpiryTime { get; set; } = null;

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; } = null!;

    }


}