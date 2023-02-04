using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RecipientTypeEntity : EnumEntity<int>
  {
    private RecipientTypeEntity() : base()
    {
    }
    private RecipientTypeEntity(RecipientType recipientType) : base((int)recipientType, recipientType.ToString())
    {
    }

    public static IEnumerable<RecipientTypeEntity> GetData() => Enum.GetValues<RecipientType>().Select(value => new RecipientTypeEntity(value));
  }
}
