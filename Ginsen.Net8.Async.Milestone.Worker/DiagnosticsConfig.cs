using System.Diagnostics;
using System.Diagnostics.Metrics;

public static class DiagnosticsConfig {
  public const string ServiceName = "ginsen-net8-async-milestone";
  public static Meter Meter = new (ServiceName);

  public static Counter<long> Counter = Meter.CreateCounter<long>("ginsen-net8-async-milestone-worker.work.count");
  public static ActivitySource ActivitySource = new (ServiceName);
}