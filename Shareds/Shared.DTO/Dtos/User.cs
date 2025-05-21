namespace Shared.DTO.Dtos
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Dni { get; set; } = null!; 
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
