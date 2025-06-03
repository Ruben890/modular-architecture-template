namespace Shared.DTO.Dtos
{
    public class User
    {
        public Guid? Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Phone { get; set; }
        public string? LastName { get; set; } = null!;
        public string? Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? Dni { get; set; } = null!;
        public int? RoleId { get; set; }
        public string? RefreshToken { get; set; } = null;
        public DateOnly? RefreshTokenExpiryTime { get; set; } = null;
        public string? RoleName { get; set; }
    }
}
