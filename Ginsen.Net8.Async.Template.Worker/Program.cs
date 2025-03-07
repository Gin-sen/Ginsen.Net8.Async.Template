using Ginsen.Net8.Async.Template.Worker;
using Serilog;
try
{
  var builder = Host.CreateApplicationBuilder(args);

  builder.Services.AddSerilog((services, lc) =>
  {
    lc.ReadFrom.Configuration(builder.Configuration)
      .Enrich.FromLogContext();
  });
  // builder.Services.AddHealthChecks();
  builder.Services.AddHostedService<Worker>();

  using var host = builder.Build();
  host.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}