namespace API.LiteratureTime.API.RouterModules;

using FluentValidation;
using Filters;
using Core.Interfaces;
using Core.Models;
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
                        request.Hour,
                        request.Minute
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
                    return literatureService.GetLiteratureTimeAsync(request.Hash);
                }
            )
            .AddEndpointFilter<ValidationFilter<LiteratureRequest>>()
            .WithName("GetLiteratureTime");
    }
}
