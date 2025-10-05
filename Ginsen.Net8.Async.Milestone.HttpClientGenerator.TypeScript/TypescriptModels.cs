using System.Collections.Generic;
namespace Ginsen.Net8.Async.Milestone.HttpClientGenerator.TypeScript;

public class RecordInfo
{
    public string Name { get; set; }
    public List<(string Name, string Type)> Properties { get; set; } = new();
}

public class EndpointInfo
{
    public string Name { get; set; }
    public string HttpMethod { get; set; }
    public string Route { get; set; }
    public string? RequestType { get; set; }
    public string? ResponseType { get; set; }
}