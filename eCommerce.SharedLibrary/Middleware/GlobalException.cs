using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Declare variables
            string message = "sorry, internal server error occured. Kindly try again";

            int statusCode = (int)HttpStatusCode.InternalServerError;

            string title = "Error";

            try
            {
                await next(context);

                // Check if exception is too many Request // 429 status code.
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too Many Requests";
                    statusCode = StatusCodes.Status429TooManyRequests;

                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Response is UnAuthorized
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Response is Forbidden // 403 Status Code
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed/required to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }

                // If Exception is caught
                // If Exception of the exceptions then do default
                await ModifyHeader(context, title, message, statusCode);
            }
            catch (Exception ex)
            {
                // Log Original Exceptions / File, Debugger, Console 
                LogException.LogExceptions(ex);

                // Check if Exception is timeOut // 408 request timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of Time";
                    message = "Request timeout... try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                await ModifyHeader(context, title, message, statusCode);

                // If none of the exceptions then do the default

            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {

                Detail = message,
                Status = statusCode,
                Title = title

            }), CancellationToken.None);

            return;
        }
    }
}
