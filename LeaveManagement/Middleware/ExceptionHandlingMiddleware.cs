using Leave.Core.Exceptions;
using System.Net;

namespace LeaveManagement.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Redirect($"/Home/Error?message={ex.Message}");
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Redirect($"/Home/Error?message={ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Redirect("/Home/Error?message=An unexpected error occurred");
            }
        }
    }
}
