namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
{
  internal class SendGridSettings
  {
    public SendGridSettings(IReadOnlyDictionary<string, string?> settings)
    {
      if (settings.TryGetValue(nameof(ApiKey), out string? apiKey) && apiKey != null)
      {
        ApiKey = apiKey;
      }
    }

    public string ApiKey { get; } = string.Empty;
  }
}
