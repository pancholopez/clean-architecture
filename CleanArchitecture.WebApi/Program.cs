using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Presentation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddPresentation();

builder.Host
    .UseSerilog((context, configuration) 
        => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapGet("/alive", () => "I am alive!")
    .WithName("alive")
    .WithOpenApi();

app.Run();