using Ginsen.Net8.Async.Milestone.Worker;
using Serilog;

var configuration = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", false, true)
  .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
  .AddEnvironmentVariables()
  .Build();

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddConfiguration(configuration);

// Configure logging
var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
var logBuilder = new LoggerConfiguration()
  .Enrich.WithThreadId()
  .Enrich.WithThreadName()
  .Enrich.WithMachineName()
  .Enrich.WithEnvironmentName()
  .Enrich.FromLogContext()
  .WriteTo.Console();

if (useOtlpExporter)
{
  logBuilder
      .WriteTo.OpenTelemetry(options =>
    {
      options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
      options.ResourceAttributes.Add("service.name", builder.Configuration["OTEL_SERVICE_NAME"] ?? "Unknown");
    });
}

Log.Logger = logBuilder.CreateLogger();
builder.Services.AddSerilog();

// builder.Services.AddHealthChecks();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await Task.Delay(100);
await host.RunAsync();

Log.Information("Stopped cleanly");