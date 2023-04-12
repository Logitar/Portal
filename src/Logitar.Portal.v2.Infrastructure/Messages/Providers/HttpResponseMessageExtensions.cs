using Logitar.Portal.v2.Core.Messages;

namespace Logitar.Portal.v2.Infrastructure.Messages.Providers;

internal static class HttpResponseMessageExtensions
{
  public static async Task<SendMessageResult> GetSendMessageResultAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
  {
    SendMessageResult result = new(capacity: 4)
    {
      [nameof(response.StatusCode)] = ((int)response.StatusCode).ToString(),
      [nameof(response.Version)] = response.Version.ToString()
    };

    if (response.ReasonPhrase != null)
    {
      result[nameof(response.ReasonPhrase)] = response.ReasonPhrase;
    }

    string? content = await response.TryReadContentAsStringAsync(cancellationToken);
    if (content != null)
    {
      result[nameof(response.Content)] = content;
    }

    return result;
  }

  public static async Task<string?> TryReadContentAsStringAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
  {
    try
    {
      return await response.Content.ReadAsStringAsync(cancellationToken);
    }
    catch (Exception)
    {
      return null;
    }
  }
}
