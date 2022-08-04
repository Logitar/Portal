using Logitar.Portal.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Client.Implementations
{
  internal static class HttpResponseMessageExtensions
  {
    private static readonly JsonSerializerOptions _options;

    static HttpResponseMessageExtensions()
    {
      _options = new();
      _options.Converters.Add(new JsonStringEnumConverter());
      _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    }

    internal static async Task HandleAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(response);

      await response.HandleBaseAsync(cancellationToken);
    }

    internal static async Task<T> HandleAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
      var data = await response.HandleBaseAsync(cancellationToken);

      if (!data.TryGetValue(nameof(response.Content), out var content) || content == null)
      {
        throw new ArgumentException($"The {nameof(response.Content)} is required.", nameof(response));
      }

      return JsonSerializer.Deserialize<T>(content, _options)
        ?? throw new ArgumentException("The content could not be deserialized.", nameof(response));
    }

    private static async Task<IReadOnlyDictionary<string, string?>> HandleBaseAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(response);

      string? content;
      try
      {
        content = await response.Content.ReadAsStringAsync(cancellationToken);
      }
      catch (Exception)
      {
        content = null;
      }

      var data = new Dictionary<string, string?>
      {
        [nameof(response.Content)] = content,
        [nameof(response.ReasonPhrase)] = response.ReasonPhrase,
        [nameof(response.StatusCode)] = ((int)response.StatusCode).ToString(),
        [nameof(response.Version)] = response.Version.ToString()
      };

      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch (Exception innerException)
      {
        var error = new Error(response.StatusCode.ToString(), description: null, data);
        throw new ErrorException(error, innerException);
      }

      return data;
    }
  }
}
