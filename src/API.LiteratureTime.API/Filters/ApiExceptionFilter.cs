using Irrbloss.Exceptions;

namespace API.LiteratureTime.API.Filters;

public class ApiExceptionFilter : IEndpointFilter
{
    public ApiExceptionFilter() { }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        try
        {
            return await next(context);
        }
        catch (ManagedResponseException exception)
        {
            return Results.Problem(exception.ProblemDetails);
        }
    }
}
