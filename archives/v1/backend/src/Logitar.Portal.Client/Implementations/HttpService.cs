using Microsoft.Extensions.Options;
using System.Collections;

namespace Logitar.Portal.Client.Implementations
{
  internal abstract class HttpService
  {
    private readonly HttpClient _client;

    protected HttpService(HttpClient client, IOptions<PortalSettings> settings)
    {
      ArgumentNullException.ThrowIfNull(client);
      ArgumentNullException.ThrowIfNull(settings);

      _client = client ?? throw new ArgumentNullException(nameof(client));
      _client.BaseAddress = new Uri(settings.Value.BaseUrl);
      _client.DefaultRequestHeaders.Add("X-API-Key", settings.Value.ApiKey);
    }

    protected async Task<T> DeleteAsync<T>(string uri, CancellationToken cancellation)
      => await ExecuteAsync<T>(HttpMethod.Delete, uri, payload: null, sessionId: null, cancellation);

    protected async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken)
      => await GetAsync<T>(uri, sessionId: null, cancellationToken);
    protected async Task<T> GetAsync<T>(string uri, Guid? sessionId, CancellationToken cancellationToken)
      => await ExecuteAsync<T>(HttpMethod.Get, uri, payload: null, sessionId, cancellationToken);

    protected async Task<T> PatchAsync<T>(string uri, CancellationToken cancellationToken)
      => await ExecuteAsync<T>(HttpMethod.Patch, uri, payload: null, sessionId: null, cancellationToken);

    protected async Task PostAsync(string uri, object payload, CancellationToken cancellationToken)
      => await PostAsync(uri, payload, sessionId: null, cancellationToken);
    protected async Task PostAsync(string uri, Guid sessionId, CancellationToken cancellationToken)
      => await PostAsync(uri, payload: null, sessionId, cancellationToken);
    protected async Task PostAsync(string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
      => await ExecuteAsync(HttpMethod.Post, uri, payload, sessionId, cancellationToken);

    protected async Task<T> PostAsync<T>(string uri, object payload, CancellationToken cancellationToken)
      => await PostAsync<T>(uri, payload, sessionId: null, cancellationToken);
    protected async Task<T> PostAsync<T>(string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
      => await ExecuteAsync<T>(HttpMethod.Post, uri, payload, sessionId, cancellationToken);

    protected async Task<T> PutAsync<T>(string uri, object payload, CancellationToken cancellationToken)
      => await PutAsync<T>(uri, payload, sessionId: null, cancellationToken);
    protected async Task<T> PutAsync<T>(string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
      => await ExecuteAsync<T>(HttpMethod.Put, uri, payload, sessionId, cancellationToken);

    protected static string GetQueryString(IReadOnlyDictionary<string, object?> parameters)
    {
      ArgumentNullException.ThrowIfNull(parameters);

      return string.Join('&', parameters.Where(x => x.Value != null).SelectMany(x =>
      {
        if (x.Value is not string && x.Value is IEnumerable enumerable)
        {
          var values = new List<string>();

          var enumerator = enumerable.GetEnumerator();
          while(enumerator.MoveNext())
          {
            var value = enumerator.Current;
            if (value != null)
            {
              values.Add($"{x.Key}={value}");
            }
          }

          return values.AsEnumerable();
        }

        return new[] { $"{x.Key}={x.Value}" };
      }));
    }

    private async Task ExecuteAsync(HttpMethod method, string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
    {
      using var response = await ExecuteBaseAsync(method, uri, payload, sessionId, cancellationToken);

      await response.HandleAsync(cancellationToken);
    }
    private async Task<T> ExecuteAsync<T>(HttpMethod method, string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
    {
      using var response = await ExecuteBaseAsync(method, uri, payload, sessionId, cancellationToken);

      return await response.HandleAsync<T>(cancellationToken);
    }
    private async Task<HttpResponseMessage> ExecuteBaseAsync(HttpMethod method, string uri, object? payload, Guid? sessionId, CancellationToken cancellationToken)
    {
      using var request = GetRequest(method, uri, payload, sessionId);

      return await _client.SendAsync(request, cancellationToken);
    }

    private static HttpRequestMessage GetRequest(HttpMethod method, string uri, object? payload, Guid? sessionId)
    {
      ArgumentNullException.ThrowIfNull(method);
      ArgumentNullException.ThrowIfNull(uri);

      var request = new HttpRequestMessage(method, new Uri($"/api/{uri.Trim('/')}", UriKind.Relative));

      if (payload != null)
      {
        request.Content = new JsonContent(payload);
      }
      if (sessionId.HasValue)
      {
        request.Headers.Add("X-Session", sessionId.Value.ToString());
      }

      return request;
    }
  }
}
