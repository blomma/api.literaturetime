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
                    [AsParameters] RandomLiteratureRequest request
                ) =>
                {
                    return literatureService.GetRandomLiteratureTimeAsync(
                        request.hour,
                        request.minute
                    );
                }
            )
            .AddEndpointFilter<ValidationFilter<RandomLiteratureRequest>>()
            .WithName("GetRandomLiteratureTime");

        group
            .MapGet(
                "/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    [AsParameters] LiteratureRequest request
                ) =>
                {
                    return literatureService.GetLiteratureTimeAsync(request.hash);
                }
            )
            .AddEndpointFilter<ValidationFilter<LiteratureRequest>>()
            .WithName("GetLiteratureTime");
    }
}
