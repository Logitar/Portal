using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Emails.Senders.Models
{
  public class SenderModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public bool IsDefault { get; set; }

    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }

    public ProviderType Provider { get; set; }
    public IEnumerable<SettingModel> Settings { get; set; } = null!;
  }
}
