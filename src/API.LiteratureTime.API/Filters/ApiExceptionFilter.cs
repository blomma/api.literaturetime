using FluentValidation;
using Irrbloss.Exceptions;

namespace API.LiteratureTime.API.Filters;

public class ApiExceptionFilter : IEndpointFilter
{
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(
                    failureGroup => failureGroup.Key,
                    failureGroup => failureGroup.ToArray()
                );

            return Results.ValidationProblem(errors);
        }
    }
}
