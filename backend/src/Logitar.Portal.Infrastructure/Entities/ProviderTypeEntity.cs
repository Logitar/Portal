using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ProviderTypeEntity : EnumEntity<int>
  {
    private ProviderTypeEntity() : base()
    {
    }
    private ProviderTypeEntity(ProviderType providerType) : base((int)providerType, providerType.ToString())
    {
    }

    public static IEnumerable<ProviderTypeEntity> GetData() => Enum.GetValues<ProviderType>().Select(value => new ProviderTypeEntity(value));
  }
}
