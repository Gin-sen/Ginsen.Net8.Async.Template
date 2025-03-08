using Ginsen.Net8.Async.Milestone.Api.Swagger;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

try
{
  var builder = WebApplication.CreateBuilder(args);
  
  builder.Services.AddSerilog((services, lc) =>
  {
    lc.ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext();
  });
  builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
  builder.Services.AddHealthChecks();
  builder.Services.AddControllers();
  builder.Services.AddProblemDetails();
  // builder.Services.AddEndpointsApiExplorer();
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
            } )
        // this enables binding ApiVersion as a endpoint callback parameter. if you don't use it, then
        // you should remove this configuration.
        .EnableApiVersionBinding();

  if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName.Equals("Docker"))
  {
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
  }
  var app = builder.Build();

  app.UseExceptionHandler();
  app.UseStatusCodePages();

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

  app.UseAuthorization();

  app.UseHealthChecks("/health");
  app.MapControllers();

  ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Initialisation");

  if (logger.IsEnabled(LogLevel.Information))
    logger.LogInformation("Starting web application");

  await app.RunAsync();
}
catch (Exception ex)
{
  if (Log.IsEnabled(Serilog.Events.LogEventLevel.Fatal))
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}