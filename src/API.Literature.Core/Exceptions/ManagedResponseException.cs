using System.Net;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Literature.Core.Exceptions;


[Serializable]
public class ManagedresponseException : Exception
{
    public ProblemDetails ProblemDetails { get; set; } = new();

    public ManagedresponseException(Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message
        };
        problemDetails.Extensions.Add("StackTrace", exception.ToString());

        ProblemDetails = problemDetails;
    }

    public ManagedresponseException(HttpStatusCode statusCode, string message, Exception? exception = null)
    {
        var problemDetails = new ProblemDetails
        {
            Title = Message,
            Status = (int)statusCode,
            Detail = exception?.Message ?? message
        };
        if (exception != null)
        {
            problemDetails.Extensions.Add("StackTrace", exception.ToString());
        }

        ProblemDetails = problemDetails;
    }

    protected ManagedresponseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}