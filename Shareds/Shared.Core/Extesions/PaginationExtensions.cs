using Microsoft.EntityFrameworkCore;
using Shared.DTO.Request;

namespace Shared.Core.Extesions
{
    public static class PaginationExtensions
    {
        public static async Task<PagedList<TEntity>> ToPaginateAsync<TEntity>(
            this IQueryable<TEntity> source,
            int pageNumber,
            int pageSize) where TEntity : class
        {
            // Validación de parámetros
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be at least 1.", nameof(pageNumber));

            if (pageSize < 1)
                throw new ArgumentException("Page size must be at least 1.", nameof(pageSize));

            // Conteo total asíncrono
            var totalCount = await source.CountAsync();

            // Obtención de los elementos paginados
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<TEntity>(items, totalCount, pageNumber, pageSize);
        }
    }
}