using Shared.DTO.Request;
using Shared.DTO.Response;
using System.Net;

namespace Shared.Core.Extensions
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Crea una ApiResponse personalizada con un mensaje, código HTTP, y detalles opcionales.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que invoca el método (puede ser cualquier tipo).</typeparam>
        /// <param name="entity">Objeto invocador (no se usa para el contenido de la respuesta).</param>
        /// <param name="message">Mensaje descriptivo para la respuesta.</param>
        /// <param name="statusCode">Código HTTP para la respuesta (por defecto BadRequest).</param>
        /// <param name="details">Objeto opcional que se asigna a la propiedad Details de la respuesta.</param>
        /// <param name="pagination">Datos opcionales de paginación.</param>
        /// <returns>Una instancia de ApiResponse configurada.</returns>
        public static ApiResponse CustomResponse<T>(this T entity, string message,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest, object? details = null, Pagination? pagination = null)
        {
            var response = new ApiResponse
            {
                Details = details,
                Message = message,
                StatusCode = statusCode
            };

            if (pagination != null)
            {
                response.SetPagination(pagination);
            }

            return response;
        }
    }
}
