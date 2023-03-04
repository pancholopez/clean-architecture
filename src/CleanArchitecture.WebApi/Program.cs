using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplication()
        .AddInfrastructure()
        .AddPresentation();

    builder.Host
        .UseSerilog((context, configuration)
            => configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithEnvironmentName());

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.MapGet("/alive", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Some info here!");
            logger.LogWarning("This is a warning!");
            logger.LogError(new Exception("error"), "This is an error!!!");
            return "I am alive!";
        })
        .WithName("alive")
        .WithOpenApi();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

// Make it visible to tests projects
public partial class Program
{
}