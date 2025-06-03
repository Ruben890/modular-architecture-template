namespace Shared.DTO.Request.QueryParameters
{
    public class GenericParameters : PaginationParameters
    {
        public Guid? UserId { get; set; } = null!;
        public Guid? CompanyId { get; set; } = null!;
        public string? ApartmentNumber { get; set; } = null!;
        public string? SearchTerm { get; set; } = null!;
    }
}
