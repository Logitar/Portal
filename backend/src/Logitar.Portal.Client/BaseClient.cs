using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Errors;
using System.Net;

namespace Logitar.Portal.Client;

internal abstract class BaseClient
{
  protected virtual JsonApiClient Client { get; }
  protected virtual JsonSerializerOptions SerializerOptions { get; }

  protected BaseClient(HttpClient client, IPortalSettings settings)
  {
    Client = new JsonApiClient(client, settings.ToHttpApiSettings());

    SerializerOptions = new();
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  protected virtual async Task<T?> DeleteAsync<T>(Uri uri, IRequestContext? context)
    => await SendAsync<T>(HttpMethod.Delete, uri, content: null, context);
  protected virtual async Task<T?> GetAsync<T>(Uri uri, IRequestContext? context)
    => await SendAsync<T>(HttpMethod.Get, uri, content: null, context);
  protected virtual async Task<T?> PatchAsync<T>(Uri uri, object? content, IRequestContext? context)
    => await SendAsync<T>(HttpMethod.Patch, uri, content, context);
  protected virtual async Task<T?> PostAsync<T>(Uri uri, object? content, IRequestContext? context)
    => await SendAsync<T>(HttpMethod.Post, uri, content, context);
  protected virtual async Task<T?> PutAsync<T>(Uri uri, object? content, IRequestContext? context)
    => await SendAsync<T>(HttpMethod.Put, uri, content, context);
  protected virtual async Task<T?> SendAsync<T>(HttpMethod method, Uri uri, object? content, IRequestContext? context)
  {
    HttpRequestParameters parameters = new(method, uri);
    if (content != null)
    {
      parameters.Content = JsonContent.Create(content, content.GetType(), mediaType: null, SerializerOptions);
    }
    if (context != null)
    {
      if (!string.IsNullOrWhiteSpace(context.User))
      {
        parameters.Headers.Add(new HttpHeader(Headers.User, context.User.Trim()));
      }
    }

    JsonApiResult result;
    try
    {
      result = await Client.SendAsync(parameters, context?.CancellationToken ?? default);
    }
    catch (HttpFailureException<JsonApiResult> exception)
    {
      result = exception.Result;
      if (result.Status.Code == (int)HttpStatusCode.NotFound)
      {
        Error? error = result.Deserialize<Error>(SerializerOptions);
        if (IsNullOrEmpty(error))
        {
          return default;
        }
      }

      throw;
    }

    return result.Deserialize<T>(SerializerOptions);
  }

  protected virtual InvalidApiResponseException CreateInvalidApiResponseException(string methodName, HttpMethod httpMethod, Uri uri, object? content, IRequestContext? context)
  {
    string? serializedContent = content == null ? null : JsonSerializer.Serialize(content, content.GetType(), SerializerOptions);
    return new InvalidApiResponseException(GetType(), methodName, httpMethod, uri, serializedContent, context);
  }

  protected virtual bool IsNullOrEmpty(Error? error) => error == null
    || (string.IsNullOrWhiteSpace(error.Code) && string.IsNullOrWhiteSpace(error.Message) && error.Data.Count == 0);
}
