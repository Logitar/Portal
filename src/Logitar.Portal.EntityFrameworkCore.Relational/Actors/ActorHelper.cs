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

  public static IEnumerable<ActorId> GetIds(params SessionEntity[] sessions)
  {
    HashSet<ActorId> ids = new(capacity: sessions.Length * 10);

    foreach (SessionEntity session in sessions)
    {
      ids.Add(new ActorId(session.CreatedBy));
      ids.Add(new ActorId(session.UpdatedBy));

      if (session.SignedOutBy != null)
      {
        ids.Add(new ActorId(session.SignedOutBy));
      }

      if (session.User != null)
      {
        ids.Add(new ActorId(session.User.CreatedBy));
        ids.Add(new ActorId(session.User.UpdatedBy));

        if (session.User.PasswordChangedBy != null)
        {
          ids.Add(new ActorId(session.User.PasswordChangedBy));
        }
        if (session.User.DisabledBy != null)
        {
          ids.Add(new ActorId(session.User.DisabledBy));
        }

        if (session.User.AddressVerifiedBy != null)
        {
          ids.Add(new ActorId(session.User.AddressVerifiedBy));
        }

        if (session.User.EmailVerifiedBy != null)
        {
          ids.Add(new ActorId(session.User.EmailVerifiedBy));
        }

        if (session.User.PhoneVerifiedBy != null)
        {
          ids.Add(new ActorId(session.User.PhoneVerifiedBy));
        }
      }
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
