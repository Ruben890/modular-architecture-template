using Mapster;

namespace Shared.DTO.Request.DTO
{
    
    public class Requestlogin
    {

        public string? Email { get; set; } = null;
        public string? UserName { get; set; } = null;
        public string? Password { get; set; } = null;
    }
}
