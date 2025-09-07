namespace BudgetExperiment.Api.Middleware;

using System.Net;
using System.Text.Json;

using BudgetExperiment.Domain;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>Converts exceptions to RFC 7807 problem details responses.</summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.</summary>
    /// <param name="next">Next request delegate.</param>
    /// <param name="logger">Logger.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    /// <summary>Invoke middleware.</summary>
    /// <param name="context">HTTP context.</param>
    /// <returns>Task.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this._next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await this.WriteProblemAsync(context, ex).ConfigureAwait(false);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        int status;
        string title;
        if (ex is DomainException de && de.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            status = (int)HttpStatusCode.NotFound;
            title = "Not Found";
        }
        else if (ex is DomainException)
        {
            status = StatusCodes.Status400BadRequest;
            title = "Domain Validation Error";
        }
        else
        {
            status = StatusCodes.Status500InternalServerError;
            title = "Internal Server Error";
            this._logger.LogError(ex, "Unhandled exception");
        }

        var problem = new
        {
            type = "about:blank",
            title,
            status,
            detail = ex.Message,
            traceId = context.TraceIdentifier,
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem)).ConfigureAwait(false);
    }
}
