using Logitar.Portal.Contracts.Errors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Client.Implementations;

internal static class HttpResponseMessageExtensions
{
  private static readonly JsonSerializerOptions _options = new();
  static HttpResponseMessageExtensions()
  {
    _options.Converters.Add(new JsonStringEnumConverter());
    _options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  internal static async Task HandleAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
  {
    await response.HandleBaseAsync(cancellationToken);
  }

  internal static async Task<T> HandleAsync<T>(this HttpResponseMessage response, CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<string, string> data = await response.HandleBaseAsync(cancellationToken);

    if (!data.TryGetValue(nameof(response.Content), out string? content) || content == null)
    {
      throw new ArgumentException($"The {nameof(response.Content)} is required.", nameof(response));
    }

    return JsonSerializer.Deserialize<T>(content, _options)
      ?? throw new ArgumentException("The content could not be deserialized.", nameof(response));
  }

  private static async Task<IReadOnlyDictionary<string, string>> HandleBaseAsync(this HttpResponseMessage response, CancellationToken cancellationToken)
  {
    List<ErrorData> data = new(capacity: 4)
    {
      new ErrorData
      {
        Key = nameof(response.StatusCode),
        Value = ((int)response.StatusCode).ToString()
      },
      new ErrorData
      {
        Key = nameof(response.Version),
        Value = response.Version.ToString()
      }
    };

    if (response.ReasonPhrase != null)
    {
      data.Add(new ErrorData
      {
        Key = nameof(response.ReasonPhrase),
        Value = response.ReasonPhrase
      });
    }

    string? content;
    try
    {
      content = await response.Content.ReadAsStringAsync(cancellationToken);
    }
    catch (Exception)
    {
      content = null;
    }
    if (content != null)
    {
      data.Add(new ErrorData
      {
        Key = nameof(response.Content),
        Value = content
      });
    }

    try
    {
      response.EnsureSuccessStatusCode();
    }
    catch (Exception innerException)
    {
      Error error = new()
      {
        Code = response.StatusCode.ToString(),
        Description = "The remote API did not return a success status code.",
        Data = data
      };
      throw new ErrorException(error, innerException);
    }

    return data.ToDictionary(x => x.Key, x => x.Value).AsReadOnly();
  }
}
