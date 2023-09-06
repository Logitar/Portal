using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.Client;

internal abstract class ClientBase
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  protected ClientBase(HttpClient client, PortalSettings settings)
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    HttpClient = client;
    HttpClient.BaseAddress = new Uri(settings.BaseUrl);
    if (settings.ApiKey != null)
    {
      HttpClient.DefaultRequestHeaders.Add(Headers.XApiKey, settings.ApiKey);
    }
    else if (settings.BasicAuthentication != null)
    {
      string authorization = string.Join(' ', Schemes.Basic, settings.BasicAuthentication.Encode());
      HttpClient.DefaultRequestHeaders.Add(Headers.Authorization, authorization);
    }

    Settings = settings;
  }

  protected HttpClient HttpClient { get; }
  protected PortalSettings Settings { get; }

  protected async Task<T?> DeleteAsync<T>(string path, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Delete, path, cancellationToken);
  protected async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Get, path, cancellationToken);
  protected async Task<T?> PatchAsync<T>(string path, CancellationToken cancellationToken)
  => await ExecuteAsync<T>(HttpMethod.Patch, path, cancellationToken);
  protected async Task<T?> PatchAsync<T>(string path, object payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Patch, path, payload, cancellationToken);
  protected async Task<T?> PostAsync<T>(string path, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Post, path, cancellationToken);
  protected async Task<T?> PostAsync<T>(string path, object payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Post, path, payload, cancellationToken);
  protected async Task<T?> PutAsync<T>(string path, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Put, path, cancellationToken);
  protected async Task<T?> PutAsync<T>(string path, object payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Put, path, payload, cancellationToken);

  protected async Task<T?> ExecuteAsync<T>(HttpMethod method, string path, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(method, path, payload: null, cancellationToken);
  protected async Task<T?> ExecuteAsync<T>(HttpMethod method, string path, object? payload, CancellationToken cancellationToken)
  {
    Uri requestUri = new(path, UriKind.Relative);
    using HttpRequestMessage request = new(method, requestUri);
    if (payload != null)
    {
      request.Content = JsonContent.Create(payload, payload.GetType(), options: _serializerOptions);
    }

    using HttpResponseMessage response = await HttpClient.SendAsync(request, cancellationToken);
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
      try
      {
        ErrorDetail? error = await response.Content.ReadFromJsonAsync<ErrorDetail>(_serializerOptions, cancellationToken);
        if (string.IsNullOrWhiteSpace(error?.ErrorCode))
        {
          return default;
        }
      }
      catch (Exception)
      {
      }
    }

    try
    {
      response.EnsureSuccessStatusCode();
    }
    catch (Exception innerException)
    {
      HttpResponseDetail detail = await HttpResponseDetail.CreateAsync(response, cancellationToken);
      throw new HttpFailureException(detail, innerException);
    }

    return await response.Content.ReadFromJsonAsync<T>(_serializerOptions, cancellationToken);
  }
}
