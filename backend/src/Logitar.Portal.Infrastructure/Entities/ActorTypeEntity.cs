using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ActorTypeEntity : EnumEntity<int>
  {
    private ActorTypeEntity() : base()
    {
    }
    private ActorTypeEntity(ActorType actorType) : base((int)actorType, actorType.ToString())
    {
    }

    public static IEnumerable<ActorTypeEntity> GetData() => Enum.GetValues<ActorType>().Select(value => new ActorTypeEntity(value));
  }
}
