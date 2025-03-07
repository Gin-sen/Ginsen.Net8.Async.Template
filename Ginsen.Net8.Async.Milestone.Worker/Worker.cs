namespace Ginsen.Net8.Async.Milestone.Worker
{
  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          if (_logger.IsEnabled(LogLevel.Debug))
          {
            _logger.LogDebug("Worker running at: {time}", DateTimeOffset.Now);
          }
          await Task.Delay(1_000 * 30, stoppingToken);
        }
        catch (Exception ex)
        {
          if (_logger.IsEnabled(LogLevel.Error))
          {
            _logger.Log(LogLevel.Error,"Worker running at: {time}", DateTimeOffset.Now);
          }
        }
      }
    }
  }
}
