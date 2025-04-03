using Ginsen.Net8.Async.Milestone.Worker;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);

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
      .WriteTo.OpenTelemetry();
}

Log.Logger = logBuilder.CreateLogger();
builder.Services.AddSerilog();


if (useOtlpExporter)
{
  builder.Services
      .AddOpenTelemetry()
      .ConfigureResource(resource => 
      {
        resource.AddService(builder.Configuration["OTEL_SERVICE_NAME"] ?? "Unknown");
      })
      .WithMetrics(metrics =>
      {
        metrics
          .AddHttpClientInstrumentation();
        /* Add more instrument here */
        metrics
          .AddMeter(DiagnosticsConfig.Meter.Name);
        metrics
          .AddOtlpExporter();
      }).WithTracing(tracing =>
    {
      tracing
        .SetSampler(new AlwaysOnSampler())
        .AddHttpClientInstrumentation();
      tracing
        .AddSource(DiagnosticsConfig.ActivitySource.Name);
      /* Add more instrument here: MassTransit, NgSql ... */
      tracing
        .AddOtlpExporter();
    });
}

// builder.Services.AddHealthChecks();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
Log.Information("Stopped cleanly");