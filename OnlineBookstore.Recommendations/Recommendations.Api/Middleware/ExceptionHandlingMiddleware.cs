﻿using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Text.Json;

namespace Recommendations.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                AuthenticationException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest,
                HttpRequestException => StatusCodes.Status400BadRequest,
                AccessViolationException => StatusCodes.Status403Forbidden,
                NullReferenceException => StatusCodes.Status404NotFound,

                _ => StatusCodes.Status500InternalServerError,
            };

            context.Response.ContentType = "application/json";

            var innerExceptionMes = string.Empty;
            if (exception.InnerException?.Message is not null)
            {
                innerExceptionMes = exception.InnerException?.Message;
            }

            var errorObject = new { error = exception.Message + innerExceptionMes };
            var errorJson = JsonSerializer.Serialize(errorObject);
            await context.Response.WriteAsync(errorJson);
        }
    }
}
