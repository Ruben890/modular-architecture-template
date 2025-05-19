using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Shared.DTO.Response;

namespace Modular_Architecture_Template.Filters
{
    public class StandardResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
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

            switch (context.Result)
            {
                case ObjectResult objectResult when objectResult is not BadRequestObjectResult:
                    var response = new ApiResponse
                    {
                        Details = objectResult.Value,
                        StatusCode = (HttpStatusCode)(objectResult.StatusCode ?? 200),
                        Message = context.ModelState.IsValid
                            ? "Operation completed successfully."
                            : "Validation error."
                    };

                    context.Result = new ObjectResult(response)
                    {
                        StatusCode = objectResult.StatusCode
                    };
                    break;

                case BadRequestObjectResult badRequestResult:
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Bad request.",
                        Details = badRequestResult.Value
                    })
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                    break;

                case NotFoundResult:
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Resource not found."
                    })
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                    break;
            }
        }
    }
}
