namespace Shared.DTO.Request.Dtos
{
    public class RequestLogin
    {
        public string? EmailOrUserName { get; set; } = null;
        public string? Password { get; set; } = null;
    }
}
