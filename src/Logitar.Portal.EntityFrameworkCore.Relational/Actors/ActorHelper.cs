using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal static class ActorHelper
{
  public static IEnumerable<ActorId> GetIds(params AggregateEntity[] aggregates)
  {
    HashSet<ActorId> ids = new(capacity: aggregates.Length * 2);

    foreach (AggregateEntity aggregate in aggregates)
    {
      ids.Add(new ActorId(aggregate.CreatedBy));
      ids.Add(new ActorId(aggregate.UpdatedBy));
    }

    return ids;
  }

  public static IEnumerable<ActorId> GetIds(params UserEntity[] users)
  {
    HashSet<ActorId> ids = new(capacity: users.Length * 7);

    foreach (UserEntity user in users)
    {
      ids.Add(new ActorId(user.CreatedBy));
      ids.Add(new ActorId(user.UpdatedBy));

      if (user.PasswordChangedBy != null)
      {
        ids.Add(new ActorId(user.PasswordChangedBy));
      }
      if (user.DisabledBy != null)
      {
        ids.Add(new ActorId(user.DisabledBy));
      }

      if (user.AddressVerifiedBy != null)
      {
        ids.Add(new ActorId(user.AddressVerifiedBy));
      }

      if (user.EmailVerifiedBy != null)
      {
        ids.Add(new ActorId(user.EmailVerifiedBy));
      }

      if (user.PhoneVerifiedBy != null)
      {
        ids.Add(new ActorId(user.PhoneVerifiedBy));
      }
    }

    return ids;
  }
}
