using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Apis.Gateway.ExceptionHandlers;

public sealed class ProblemDetailsExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProblemDetailsExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ProblemDetailsException problemDetailsException)
        {
            return new ValueTask<bool>(true);
        }

        var problemDetails = new ProblemDetails
        {
            Title = problemDetailsException.Title,
            Detail = problemDetailsException.Message,
            Status = (int)problemDetailsException.StatusCode,
        };

        return _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}

[Serializable]
public sealed class ProblemDetailsException : Exception
{
    public string Title { get; init; }
    public string Message { get; init; }
    public HttpStatusCode StatusCode { get; init; }

    private ProblemDetailsException(string title, string message, HttpStatusCode statusCode) : base(message)
    {
        Title = title;
        Message = message;
    }

    public static void Throw([NotNull] string title, [NotNull] string message, [NotNull] HttpStatusCode statusCode)
    {
        throw new ProblemDetailsException(title, message, statusCode);
    }
}
