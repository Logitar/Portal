namespace Logitar.Portal.Application.Senders;

internal static class MailgunHelper
{
  public static string GenerateApiKey() => string.Concat("key-", Guid.NewGuid().ToString("n"));
}
