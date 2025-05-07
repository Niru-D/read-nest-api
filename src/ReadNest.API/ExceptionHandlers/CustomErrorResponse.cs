using Microsoft.AspNetCore.Mvc;

namespace ReadNest.API.ExceptionHandlers
{
    /// <summary>
    /// Provides a centralized way to generate standardized error responses for API controllers.
    /// </summary>
    public static class CustomErrorResponse
    {
        /// <summary>
        /// Generates a standardized error response based on the provided status code and message.
        /// </summary>
        /// <param name="controller">The controller from which the response is being sent</param>
        /// <param name="statusCode">The HTTP status code for the response</param>
        /// <param name="message">The error message to include in the response</param>
        /// <returns>An appropriate IActionResult based on the status code</returns>
        public static IActionResult ErrorResponse(this ControllerBase controller, int statusCode, string message)
        {
            var problemDetails = new ProblemDetails
            {
                Type = GetDefaultType(statusCode),
                Status = statusCode,
                Title = GetDefaultTitle(statusCode),
                Detail = message
            };

            return statusCode switch
            {
                StatusCodes.Status400BadRequest => controller.BadRequest(problemDetails),
                StatusCodes.Status401Unauthorized => controller.Unauthorized(problemDetails),
                StatusCodes.Status403Forbidden => controller.Forbid(),
                StatusCodes.Status404NotFound => controller.NotFound(problemDetails),
                _ => controller.StatusCode(statusCode, problemDetails)
            };
        }

        private static string GetDefaultTitle(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            _ => "Error"
        };

        private static string GetDefaultType(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            StatusCodes.Status401Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            StatusCodes.Status403Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }
}
