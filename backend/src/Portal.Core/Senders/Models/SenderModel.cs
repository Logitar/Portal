using Portal.Core.Realms.Models;

namespace Portal.Core.Senders.Models
{
  public class SenderModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public bool IsDefault { get; private set; }

    public string EmailAddress { get; set; } = null!;
    public string? DisplayName { get; set; }

    public ProviderType Provider { get; set; }
    public IEnumerable<SettingModel> Settings { get; set; } = null!;
  }
}
