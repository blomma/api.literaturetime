using System.Globalization;
using Irrbloss.Extensions;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel
    .Override("Microsoft", LogEventLevel.Information)
    .Enrich
    .FromLogContext()
    .WriteTo
    .Console(
        LogEventLevel.Verbose,
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        CultureInfo.CurrentCulture
    )
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder
    .Host
    .UseSerilog(
        (context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services)
    );

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServiceModules(builder.Configuration);
builder.Services.AddRouterModules();

builder.Services.AddMemoryCache();

builder
    .Services
    .AddHttpLogging(logging =>
    {
        logging.RequestHeaders.Add("Referer");
        logging.RequestHeaders.Add("X-Forwarded-For");
        logging.RequestHeaders.Add("X-Forwarded-Host");
        logging.RequestHeaders.Add("X-Forwarded-Port");
        logging.RequestHeaders.Add("X-Forwarded-Proto");
        logging.RequestHeaders.Add("X-Forwarded-Server");
        logging.RequestHeaders.Add("X-Real-Ip");
        logging.RequestHeaders.Add("Upgrade-Insecure-Requests");
        logging.RequestHeaders.Add("traceparent");
    });

var app = builder.Build();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouterModules();

app.Run();
