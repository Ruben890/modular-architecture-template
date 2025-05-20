using System.ComponentModel.DataAnnotations;

namespace Module.User.Domain.Entity
{

    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public virtual ICollection<User>? Users { get; set; } = null;
    }
}
