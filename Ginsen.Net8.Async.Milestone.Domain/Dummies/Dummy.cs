namespace Ginsen.Net8.Async.Milestone.Domain.Dummies;

/// <summary>
/// Domain class for a Dummy
/// </summary>
public class Dummy
{

    /// <summary>
    /// Gets or sets the partition key.
    /// </summary>
    public Guid PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the row key.
    /// </summary>
    public Guid RowKey { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string? Message { get; set; }

    public Dummy()
    {
      PartitionKey = Guid.NewGuid();
      RowKey = Guid.NewGuid();
    }
    public Dummy(string? message)
    {
      PartitionKey = Guid.NewGuid();
      RowKey = Guid.NewGuid();
      Message = message;
    }
    public Dummy(string partitionKey, string rowKey, string? message)
    {
      PartitionKey = Guid.Parse(partitionKey);
      RowKey = Guid.Parse(rowKey);
      Message = message;
    }
}
