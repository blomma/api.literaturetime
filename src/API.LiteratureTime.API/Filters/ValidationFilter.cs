using FluentValidation;

namespace API.LiteratureTime.API.Filters;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        if (context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) is not T obj)
        {
            return Results.BadRequest();
        }

        await validator.ValidateAndThrowAsync(obj);

        return await next(context);
    }
}
