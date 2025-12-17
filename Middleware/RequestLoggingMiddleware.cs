using System.Diagnostics;

namespace ConsultantManagementApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request details
        _logger.LogInformation(
            "Request started: {RequestId} {Method} {Path}",
            requestId,
            context.Request.Method,
            context.Request.Path);

        // Capture original response stream
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "Request completed: {RequestId} {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMilliseconds}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // Log response body for debugging (limit to first 1000 chars in development)
                if (context.Request.Method != "GET" || context.Response.StatusCode >= 400)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(responseBody))
                    {
                        var body = await reader.ReadToEndAsync();
                        var truncatedBody = body.Length > 1000 ? body.Substring(0, 1000) + "..." : body;
                        _logger.LogDebug("Response body: {ResponseBody}", truncatedBody);
                    }
                }

                // Copy the response back to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}
