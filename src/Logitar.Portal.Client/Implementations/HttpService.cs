using System.Collections;
using System.Net.Http.Json;

namespace Logitar.Portal.Client.Implementations;

internal abstract class HttpService : IDisposable
{
  private readonly HttpClient _client;

  protected HttpService(HttpClient client, PortalSettings settings)
  {
    _client = client;
    _client.BaseAddress = new Uri(settings.BaseUrl);
  }

  public void Dispose()
  {
    _client.Dispose();
  }

  protected async Task<T> DeleteAsync<T>(string uri, CancellationToken cancellation)
    => await ExecuteAsync<T>(HttpMethod.Delete, uri, payload: null, cancellation);

  protected async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Get, uri, payload: null, cancellationToken);

  protected async Task<T> PatchAsync<T>(string uri, CancellationToken cancellationToken)
    => await PatchAsync<T>(uri, payload: null, cancellationToken);
  protected async Task<T> PatchAsync<T>(string uri, object? payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Patch, uri, payload, cancellationToken);

  protected async Task PostAsync(string uri, CancellationToken cancellationToken)
    => await PostAsync(uri, payload: null, cancellationToken);
  protected async Task PostAsync(string uri, object? payload, CancellationToken cancellationToken)
    => await ExecuteAsync(HttpMethod.Post, uri, payload, cancellationToken);

  protected async Task<T> PostAsync<T>(string uri, object? payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Post, uri, payload, cancellationToken);

  protected async Task<T> PutAsync<T>(string uri, object? payload, CancellationToken cancellationToken)
    => await ExecuteAsync<T>(HttpMethod.Put, uri, payload, cancellationToken);

  protected static string GetQueryString(IReadOnlyDictionary<string, object?> parameters)
  {
    return string.Join('&', parameters.Where(x => x.Value != null).SelectMany(x =>
    {
      if (x.Value is not string && x.Value is IEnumerable enumerable)
      {
        List<string> values = new();

        IEnumerator enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
          object? value = enumerator.Current;
          if (value != null)
          {
            values.Add(string.Join('=', x.Key, x.Value));
          }
        }

        return values.AsEnumerable();
      }

      return new[] { string.Join('=', x.Key, x.Value) };
    }));
  }

  private async Task ExecuteAsync(HttpMethod method, string uri, object? payload, CancellationToken cancellationToken)
  {
    using HttpResponseMessage response = await ExecuteBaseAsync(method, uri, payload, cancellationToken);

    await response.HandleAsync(cancellationToken);
  }
  private async Task<T> ExecuteAsync<T>(HttpMethod method, string uri, object? payload, CancellationToken cancellationToken)
  {
    using HttpResponseMessage response = await ExecuteBaseAsync(method, uri, payload, cancellationToken);

    return await response.HandleAsync<T>(cancellationToken);
  }
  private async Task<HttpResponseMessage> ExecuteBaseAsync(HttpMethod method, string uri, object? payload, CancellationToken cancellationToken)
  {
    using HttpRequestMessage request = GetRequest(method, uri, payload);

    return await _client.SendAsync(request, cancellationToken);
  }

  private static HttpRequestMessage GetRequest(HttpMethod method, string uri, object? payload)
  {
    Uri requestUri = new($"/api/{uri.Trim('/')}", UriKind.Relative);
    HttpRequestMessage request = new(method, requestUri);

    if (payload != null)
    {
      request.Content = JsonContent.Create(payload);
    }

    return request;
  }
}
