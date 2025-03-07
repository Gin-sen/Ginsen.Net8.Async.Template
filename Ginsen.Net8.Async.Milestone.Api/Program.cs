using Serilog;

try
{
  var builder = WebApplication.CreateBuilder(args);

  builder.Services.AddSerilog((services, lc) =>
  {
    lc.ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext();
  });
  builder.Services.AddHealthChecks();
  builder.Services.AddControllers();
  builder.Services.AddProblemDetails();

  if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName.Equals("Docker"))
  {
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
  }
  var app = builder.Build();

  app.UseExceptionHandler();
  app.UseStatusCodePages();

  if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Docker"))
  {
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
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