using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Mpdules.User.Infrastrutucture
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [AllowNull]
        public string? UserName { get; set; }

        [AllowNull]
        public string? LastName { get; set; } = null!;
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [AllowNull]
        public string? Dni { get; set; } = null;

        [AllowNull]
        public string? RefreshToken { get; set; } = null;

        public DateTime CreationDate { get; set; } = DateTime.Now;

    }
}
