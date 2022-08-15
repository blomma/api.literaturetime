using API.Literature.Core.Interfaces;
using Irrbloss.Extensions;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServiceModules();
builder.Services.AddRouterModules();

builder.Services.AddMemoryCache();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                            // .WithOrigins("https://localhost:44472")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                      });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Populate cache
IMemoryCache cache = app.Services.GetRequiredService<IMemoryCache>();
using (var scope = app.Services.CreateScope())
{
    ILiteratureService literatureService = scope.ServiceProvider.GetRequiredService<ILiteratureService>();

    var literatureTimes = literatureService.GetLiteratureTimes();
    ILookup<string, API.Literature.Core.Models.LiteratureTime> lookup = literatureTimes.ToLookup(o => o.Time);

    foreach (IGrouping<string, API.Literature.Core.Models.LiteratureTime> literatureTimesGroup in lookup)
    {
        cache.Set(literatureTimesGroup.Key, literatureTimesGroup.ToList());
    }
}
// END POPULATION

app.MapRouterModules();

// app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

app.Run();
