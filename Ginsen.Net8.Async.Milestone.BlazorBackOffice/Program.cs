using Ginsen.Net8.Async.Milestone.BlazorBackOffice.Components;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
      .WriteTo.OpenTelemetry(options =>
    {
      options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
      options.ResourceAttributes.Add("service.name", builder.Configuration["OTEL_SERVICE_NAME"] ?? "Unknown");
    });
}

Log.Logger = logBuilder.CreateLogger();

builder.Services.AddSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

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