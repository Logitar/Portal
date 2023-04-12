namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid
{
  internal class SendGridSettings
  {
    public SendGridSettings(IReadOnlyDictionary<string, string?> settings)
    {
      ArgumentNullException.ThrowIfNull(settings);

      if (settings.TryGetValue(nameof(ApiKey), out var apiKey) && apiKey != null)
      {
        ApiKey = apiKey;
      }
    }

    public string ApiKey { get; } = null!;
  }
}
