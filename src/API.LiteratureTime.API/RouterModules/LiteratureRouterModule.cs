namespace API.LiteratureTime.API.RouterModules;

using FluentValidation;
using global::API.LiteratureTime.API.Filters;
using global::API.LiteratureTime.Core.Interfaces;
using global::API.LiteratureTime.Core.Models;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/literature").AddEndpointFilter<ApiExceptionFilter>();

        group
            .MapGet(
                "/{hour}/{minute}",
                (
                    [FromServices] ILiteratureService literatureService,
                    [FromServices] IValidator<RandomLiteratureRequest> validator,
                    [AsParameters] RandomLiteratureRequest request
                ) =>
                {
                    validator.ValidateAndThrow(request);

                    return literatureService.GetRandomLiteratureTimeAsync(
                        request.hour,
                        request.minute
                    );
                }
            )
            .WithName("GetRandomLiteratureTime");

        group
            .MapGet(
                "/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    [FromServices] IValidator<LiteratureRequest> validator,
                    [AsParameters] LiteratureRequest request
                ) =>
                {
                    validator.ValidateAndThrow(request);

                    return literatureService.GetLiteratureTimeAsync(request.hash);
                }
            )
            .WithName("GetLiteratureTime");
    }
}
