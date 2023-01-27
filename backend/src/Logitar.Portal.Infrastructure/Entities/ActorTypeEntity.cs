using Logitar.Portal.Core.Actors;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ActorTypeEntity : EnumEntity<int>
  {
    public ActorTypeEntity(ActorType actorType) : base((int)actorType, actorType.ToString())
    {
    }
    private ActorTypeEntity() : base()
    {
    }

    public static IEnumerable<ActorTypeEntity> GetData() => Enum.GetValues<ActorType>()
      .Select(actorType => new ActorTypeEntity(actorType));
  }
}
