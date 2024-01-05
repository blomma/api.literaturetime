namespace API.LiteratureTime.API.RouterModules;

using Core.Interfaces;
using Core.Models;
using Filters;
using FluentValidation;
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
                ) => literatureService.GetRandomLiteratureTimeAsync(request.Hour, request.Minute)
            )
            .AddEndpointFilter<ValidationFilter<RandomLiteratureRequest>>()
            .WithName("GetRandomLiteratureTime");

        group
            .MapGet(
                "/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    [AsParameters] LiteratureRequest request
                ) => literatureService.GetLiteratureTimeAsync(request.Hash)
            )
            .AddEndpointFilter<ValidationFilter<LiteratureRequest>>()
            .WithName("GetLiteratureTime");
    }
}
