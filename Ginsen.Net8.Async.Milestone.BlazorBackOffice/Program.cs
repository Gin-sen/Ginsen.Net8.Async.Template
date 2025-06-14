using Ginsen.Net8.Async.Milestone.BlazorBackOffice.Components;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

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
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
          /* Add more instrument here */

          metrics
            .AddOtlpExporter();
      }).WithTracing(tracing =>
    {
        tracing
          .SetErrorStatusOnException()
          .SetSampler(new AlwaysOnSampler())
          .AddAspNetCoreInstrumentation(options =>
          {
              options.RecordException = true;
          })
          .AddHttpClientInstrumentation();
        /* Add more instrument here: MassTransit, NgSql ... */
        tracing
          .AddOtlpExporter();
    });
}
// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();
builder.Services.AddHttpClient("Api_ServerSide", o => {
  o.BaseAddress = new Uri(builder.Configuration["HttpServices:Api_ServerSide:BaseUrl"] ?? "https://localhost:8080/");
});
builder.Services.AddScoped<IAzureTablesHttpRepository, AzureTablesHttpRepository>();

// Remove default header (security issue)
builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
// Add HealthChecks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseHealthChecks("/health");
app.MapRazorComponents<App>()
  .AddInteractiveServerRenderMode();

await app.RunAsync();
Log.Information("Stopped cleanly");