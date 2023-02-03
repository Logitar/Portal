using System.Collections.Generic;

namespace Logitar.Portal.Contracts.Senders
{
  public record CreateSenderPayload
  {
    public string? Realm { get; set; }

    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    public ProviderType Provider { get; set; }
    public IEnumerable<SettingPayload>? Settings { get; set; }
  }
}
