using System.Text;
using Ginsen.Net8.Async.Milestone.Api.Contracts.V1.Dummies;

public class AzureTablesHttpRepository : IAzureTablesHttpRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AzureTablesHttpRepository> _logger;

    public AzureTablesHttpRepository(IHttpClientFactory httpClientFactory, ILogger<AzureTablesHttpRepository> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

  public async Task UpsertAsync(CreateDummyRequest entity)
  {
    using var httpClient = _httpClientFactory.CreateClient("Api_ServerSide");
    var requestUriSb = new StringBuilder();
    requestUriSb.Append("/api/v1/azuretables");
    requestUriSb.Append('/').Append(Guid.NewGuid()).Append('/').Append(Guid.NewGuid());
    try {
      var response = await httpClient.PostAsJsonAsync<CreateDummyRequest>(requestUriSb.ToString(), entity);
      response.EnsureSuccessStatusCode();
    } catch (HttpRequestException ex) {
      _logger.LogWarning("CreateDummyRequest failed: {ExceptionMessage}", ex.Message);
      throw;
    } catch (Exception ex) {
      _logger.LogError(ex, "Unexpected error: {ExceptionMessage}", ex.Message);
      throw;
    }
  }

  public async Task<GetDummiesResult> GetAllAsync(string? partitionKey = "*", string? rowKey = "*")
    {
      using var httpClient = _httpClientFactory.CreateClient("Api_ServerSide");
      var requestUriSb = new StringBuilder();
      requestUriSb.Append("/api/v1/azuretables");
      if (partitionKey != null && rowKey != null) {
        requestUriSb.Append('?').Append("partitionKey").Append('=').Append(System.Web.HttpUtility.UrlEncode(partitionKey))
        .Append('&').Append("rowKey").Append('=').Append(System.Web.HttpUtility.UrlEncode(rowKey));
      }
      else if (partitionKey != null) {
        requestUriSb.Append('?').Append("partitionKey").Append('=').Append(System.Web.HttpUtility.UrlEncode(partitionKey));
      } else if (rowKey != null) {
        requestUriSb.Append('?').Append("rowKey").Append('=').Append(System.Web.HttpUtility.UrlEncode(rowKey));
      }
      try {
        var response = await httpClient.GetFromJsonAsync<GetDummiesResult>(requestUriSb.ToString());
        return response ?? new GetDummiesResult(Array.Empty<GetDummyResult>());
      } catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) {
        _logger.LogInformation("No GetDummyResult found");
        return new GetDummiesResult(Array.Empty<GetDummyResult>());
      } catch (Exception ex) {
        _logger.LogError(ex, "Unexpected error: {ExceptionMessage}", ex.Message);
        throw;
      }
    }

  public Task<GetDummyResult> GetByIdAsync(string id)
  {
    throw new NotImplementedException();
  }
}