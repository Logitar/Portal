namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid;

internal record SendGridSettings
{
  public SendGridSettings(IReadOnlyDictionary<string, string> settings)
  {
    if (settings.TryGetValue(nameof(ApiKey), out string? apiKey))
    {
      ApiKey = apiKey;
    }
  }

  public string ApiKey { get; } = string.Empty;
}
