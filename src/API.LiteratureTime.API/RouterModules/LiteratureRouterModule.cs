using API.LiteratureTime.API.Filters;
using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Core.Models;
using FluentValidation;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.LiteratureTime.API.RouterModules;

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
    }
}
