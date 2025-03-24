using Azure.Data.Tables;
using Azure;
using Ginsen.Net8.Async.Milestone.Domain.Dummies;

namespace Ginsen.Net8.Async.Milestone.Infrastructure.AzureStorageAccount.Entities
{
  /// <summary>
  /// Represents a dummy entity.
  /// </summary>
  public class DummyEntity : ITableEntity
  {
    /// <summary>
    /// Gets or sets the partition key.
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the row key.
    /// </summary>
    public string RowKey { get; set; }

    /// <summary>
    /// Gets or sets the ETag.
    /// </summary>
    public ETag ETag { get; set; }

    /// <summary>
    /// /// Gets or sets the timestamp.
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string? Message { get; set; }


    public DummyEntity(Dummy dummy)
    {
      PartitionKey = dummy.PartitionKey.ToString();
      RowKey = dummy.RowKey.ToString();
      Message = dummy.Message;
    }
    public DummyEntity(string partitionKey, string rowKey)
    {
      PartitionKey = partitionKey;
      RowKey = rowKey;
    }
    public DummyEntity(string partitionKey, string rowKey, string? message)
    {
      PartitionKey = partitionKey;
      RowKey = rowKey;
      Message = message;
    }
  }
}