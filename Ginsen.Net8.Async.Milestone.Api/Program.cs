using Ginsen.Net8.Async.Milestone.Api.Swagger;
using Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ginsen.Net8.Async.Milestone.Application.Repositories;
using Ginsen.Net8.Async.Milestone.Api.Endpoints.V1;
using Asp.Versioning.Builder;
using Asp.Versioning;
using Serilog.Events;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry;
using OpenTelemetry.Trace;

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
// Remove default header (security issue)
builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
// Add HealthChecks
builder.Services.AddHealthChecks();
// Add controllers
// builder.Services.AddControllers();
// Add ProblemDetails (https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-8.0#problem-details-service)
builder.Services.AddProblemDetails();
// Add API versioning for openapi generation
builder.Services.AddApiVersioning(
    options =>
    {
      // reporting api versions will return the headers
      // "api-supported-versions" and "api-deprecated-versions"
      options.ReportApiVersions = true;
    } )
  .AddApiExplorer(
    options =>
    {
      // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
      // note: the specified format code will format the version as "'v'major[.minor][-status]"
      options.GroupNameFormat = "'v'VVV";

      // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
      // can also be used to control the format of the API version in route templates
      options.SubstituteApiVersionInUrl = true;
    } );
// Add Swagger Gen only in development
if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName.Equals("Docker"))
{
  builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
}
// Add Azure Table Service
builder.Services.AddAzureClients(clientsBuilder =>
{
  clientsBuilder.AddTableServiceClient(builder.Configuration.GetConnectionString("StorageAccount"));
});

// Add services
builder.Services.TryAddSingleton<IDummiesService, DummiesService>();
builder.Services.TryAddSingleton<IDummiesRepository, DummiesRepository>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Add ProblemDetails (https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-8.0#problem-details-service)
app.UseExceptionHandler();
app.UseStatusCodePages();

// Add DeveloperExceptionPage Swagger UI only in development
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Docker"))
{
  app.UseDeveloperExceptionPage();
  app.UseSwagger();
  app.UseSwaggerUI(
      options =>
      {
          var descriptions = app.DescribeApiVersions();
      
          // build a swagger endpoint for each discovered API version
          foreach ( var description in descriptions )
          {
              var url = $"/swagger/{description.GroupName}/swagger.json";
              var name = description.GroupName.ToUpperInvariant();
              options.SwaggerEndpoint( url, name );
          }
      } );
}
// Add Authorization
app.UseAuthorization();
// Add HealthChecks to route /health
app.UseHealthChecks("/health");
// Map Endpoints
// app.MapControllers();
ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    // .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();
AzureTableEndpoints.MapAzureTableEndpoints(app, apiVersionSet);
MathEndpoints.MapMathEndpoints(app, apiVersionSet);

if (Log.IsEnabled(LogEventLevel.Information))
  Log.Information("Initialisation");

TableServiceClient tableServiceClient = app.Services.GetRequiredService<TableServiceClient>();

if (Log.IsEnabled(LogEventLevel.Information))
  Log.Information("Creating table Dummies if not exists");

try
{
  // // .NET Diagnostics: create a manual span
  // using (var activity = activitySource.StartActivity("CreateTableIfNotExiste"))
  // {
  //     activity?.SetTag("TableName", "Dummies");

      await tableServiceClient.CreateTableIfNotExistsAsync("Dummies");

  //     activity?.SetStatus(ActivityStatusCode.Ok);
  //     // .NET Diagnostics: update the metric
  //     createTableIfNotExistCounter.Add(1);
  // }
}
catch (Exception ex)
{
  if (Log.IsEnabled(LogEventLevel.Fatal))
    Log.Fatal(ex, "Error creating table Dummies");
  throw;
}

if (Log.IsEnabled(LogEventLevel.Information))
  Log.Information("Starting web application");

await app.RunAsync();

Log.Information("Stopped cleanly");
Log.CloseAndFlush();