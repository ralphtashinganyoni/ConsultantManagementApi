using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ConsultantManagementApi.Middleware;

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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var response = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        // Add specific handling for different exception types
        switch (exception)
        {
            case ArgumentNullException argNullEx:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid Request";
                response.Detail = $"Required parameter is null: {argNullEx.ParamName}";
                break;

            case ArgumentException argEx:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid Argument";
                response.Detail = argEx.Message;
                break;

            case InvalidOperationException:
                response.Status = StatusCodes.Status409Conflict;
                response.Title = "Operation Not Allowed";
                break;

            case KeyNotFoundException:
                response.Status = StatusCodes.Status404NotFound;
                response.Title = "Resource Not Found";
                break;
        }

        context.Response.StatusCode = response.Status ?? StatusCodes.Status500InternalServerError;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return context.Response.WriteAsJsonAsync(response, jsonOptions);
    }
}
