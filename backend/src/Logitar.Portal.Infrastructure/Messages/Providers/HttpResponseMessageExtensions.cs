using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages.Providers
{
  internal static class HttpResponseMessageExtensions
  {
    public static async Task<SendMessageResult> GetSendMessageResultAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
      string? content = await response.TryReadContentAsStringAsync(cancellationToken);

      return new SendMessageResult
      {
        [nameof(response.Content)] = content,
        [nameof(response.ReasonPhrase)] = response.ReasonPhrase,
        [nameof(response.StatusCode)] = ((int)response.StatusCode).ToString(),
        [nameof(response.Version)] = response.Version.ToString()
      };
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
}
