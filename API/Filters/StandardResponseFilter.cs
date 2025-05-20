using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.DTO.Response;
using System.Net;

public class StandardResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Manejo de excepción global
        if (context.Exception != null)
        {
            var errorResponse = new ApiResponse
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred.",
                Details = context.Exception.Message
            };

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
            return;
        }

        if (context.Result is ObjectResult objectResult)
        {
            // Si el servicio ya devolvió ApiResponse, se respeta totalmente
            if (objectResult.Value is ApiResponse apiResponse)
            {
                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = (int)apiResponse.StatusCode
                };
                return;
            }

            // Si el servicio no devolvió ApiResponse (caso raro), se envuelve
            var statusCode = objectResult.StatusCode ?? (int)HttpStatusCode.OK;
            var isString = objectResult.Value is string;

            var wrapped = new ApiResponse
            {
                StatusCode = (HttpStatusCode)statusCode,
                Message = isString ? objectResult.Value?.ToString() : "Operation completed successfully.",
                Details = isString ? null : objectResult.Value
            };

            context.Result = new ObjectResult(wrapped)
            {
                StatusCode = statusCode
            };
        }
    }
}
