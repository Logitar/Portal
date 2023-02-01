using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ActorEntity
  {
    public ActorEntity(UserCreatedEvent @event)
    {
      Type = ActorType.User;
      AggregateId = @event.AggregateId.Value;

      DisplayName = @event.FullName ?? @event.Username;
      Email = @event.Email;
      Picture = @event.Picture;
    }
    private ActorEntity()
    {
    }

    public int ActorId { get; private set; }

    public ActorType Type { get; private set; }
    public string AggregateId { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Picture { get; private set; }
  }
}
