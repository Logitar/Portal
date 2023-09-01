using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal static class ActorExtensions
{
  public static IEnumerable<ActorId> GetActorIds(this AggregateRoot aggregate)
  {
    return new[] { aggregate.CreatedBy, aggregate.UpdatedBy }.Distinct();
  }

  public static IEnumerable<ActorId> GetActorIds(this AggregateEntity aggregate)
  {
    return new[] { new ActorId(aggregate.CreatedBy), new ActorId(aggregate.UpdatedBy) }.Distinct();
  }

  public static IEnumerable<ActorId> GetActorIds(this ApiKeyEntity apiKey)
  {
    List<ActorId> actorIds = new(capacity: 2 + (2 * apiKey.Roles.Count))
    {
      new ActorId(apiKey.CreatedBy),
      new ActorId(apiKey.UpdatedBy)
    };

    foreach (RoleEntity role in apiKey.Roles)
    {
      actorIds.AddRange(role.GetActorIds());
    }

    return actorIds.Distinct();
  }

  public static IEnumerable<ActorId> GetActorIds(this IdentifierEntity identifier)
  {
    return new[] { new ActorId(identifier.CreatedBy), new ActorId(identifier.UpdatedBy) };
  }

  public static IEnumerable<ActorId> GetActorIds(this SessionEntity session)
  {
    List<ActorId> actorIds = new(capacity: 10)
    {
      new ActorId(session.CreatedBy),
      new ActorId(session.UpdatedBy)
    };

    if (session.SignedOutBy != null)
    {
      actorIds.Add(new ActorId(session.SignedOutBy));
    }

    if (session.User != null)
    {
      actorIds.AddRange(GetActorIds(session.User));
    }

    return actorIds.Distinct();
  }

  public static IEnumerable<ActorId> GetActorIds(this UserEntity user)
  {
    int capacity = 7 + (2 * user.Identifiers.Count) + (2 * user.Roles.Count);
    List<ActorId> actorIds = new(capacity)
    {
      new ActorId(user.CreatedBy),
      new ActorId(user.UpdatedBy)
    };

    if (user.PasswordChangedBy != null)
    {
      actorIds.Add(new ActorId(user.PasswordChangedBy));
    }
    if (user.DisabledBy != null)
    {
      actorIds.Add(new ActorId(user.DisabledBy));
    }
    if (user.AddressVerifiedBy != null)
    {
      actorIds.Add(new ActorId(user.AddressVerifiedBy));
    }
    if (user.EmailVerifiedBy != null)
    {
      actorIds.Add(new ActorId(user.EmailVerifiedBy));
    }
    if (user.PhoneVerifiedBy != null)
    {
      actorIds.Add(new ActorId(user.PhoneVerifiedBy));
    }

    foreach (UserIdentifierEntity identifier in user.Identifiers)
    {
      actorIds.AddRange(identifier.GetActorIds());
    }

    foreach (RoleEntity role in user.Roles)
    {
      actorIds.AddRange(role.GetActorIds());
    }

    return actorIds.Distinct();
  }
}
