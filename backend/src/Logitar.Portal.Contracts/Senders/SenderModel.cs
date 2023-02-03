using Logitar.Portal.Contracts.Realms;
using System.Collections.Generic;
using System.Linq;

namespace Logitar.Portal.Contracts.Senders
{
  public record SenderModel : AggregateModel
  {
    public RealmModel? Realm { get; set; }

    public bool IsDefault { get; set; }

    public string EmailAddress { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    public ProviderType Provider { get; set; }
    public IEnumerable<SettingModel> Settings { get; set; } = Enumerable.Empty<SettingModel>();
  }
}
