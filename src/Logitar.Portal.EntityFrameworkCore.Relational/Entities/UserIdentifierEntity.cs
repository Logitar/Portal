using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record UserIdentifierEntity : IdentifierEntity
{
  public UserIdentifierEntity(UserIdentifierSetEvent @event, UserEntity user) : base(@event, user.TenantId)
  {
    User = user;
    UserId = user.UserId;
  }

  private UserIdentifierEntity()
  {
  }

  public int UserIdentifierId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public void Update(UserIdentifierSetEvent @event) => Update(@event);
}
