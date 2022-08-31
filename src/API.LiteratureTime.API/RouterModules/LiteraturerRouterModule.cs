namespace API.LiteratureTime.API.RouterModules;

using global::API.LiteratureTime.Core.Interfaces;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/1.0/literature/{hour}/{minute}", ([FromServices] ILiteratureService literatureService, string hour, string minute) =>
        {
            return literatureService.GetRandomLiteratureTime(hour, minute);
        })
        .WithName("GetRandomLiteratureTime");

        app.MapGet("/api/1.0/literature/{hour}/{minute}/{hash}", ([FromServices] ILiteratureService literatureService, string hour, string minute, string hash) =>
        {
            return literatureService.GetLiteratureTime(hour, minute, hash);
        })
        .WithName("GetSpecificLiteratureTime");

        app.MapGet("/api/1.0/literatures", ([FromServices] ILiteratureService literatureService) =>
        {
            return literatureService.GetLiteratureTimes();
        })
        .WithName("GetLiteratureTimes");
    }
}