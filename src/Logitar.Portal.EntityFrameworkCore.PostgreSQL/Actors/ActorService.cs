using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;
  private readonly PortalContext _context;

  public ActorService(ICacheService cacheService, PortalContext context)
  {
    _cacheService = cacheService;
    _context = context;
  }
  public async Task<ActorEntity> GetAsync(DomainEvent e, CancellationToken cancellationToken)
  {
    Guid id = e.ActorId.ToGuid();

    if (id == Guid.Empty)
    {
      return ActorEntity.System;
    }

    Actor? actor = _cacheService.GetActor(id);
    if (actor == null)
    {
      UserEntity? user = await _context.Users
        .SingleOrDefaultAsync(x => x.AggregateId == e.ActorId.Value, cancellationToken);
      if (user != null)
      {
        actor = ActorEntity.From(user).ToActor(id);
      }

      if (actor == null)
      {
        throw new InvalidOperationException($"The actor '{id}' could not be found.");
      }

      _cacheService.SetActor(actor);
    }

    return ActorEntity.From(actor);
  }

  public async Task DeleteAsync(UserEntity user, CancellationToken cancellationToken = default)
  {
    await SetActorAsync(user.AggregateId, ActorEntity.From(user, isDeleted: true), cancellationToken);
  }
  public async Task UpdateAsync(UserEntity user, CancellationToken cancellationToken)
  {
    await SetActorAsync(user.AggregateId, ActorEntity.From(user), cancellationToken);
  }

  private async Task SetActorAsync(string aggregateId, ActorEntity actor, CancellationToken cancellationToken)
  {
    Guid id = new AggregateId(aggregateId).ToGuid();

    if (actor.IsDeleted)
    {
      _cacheService.RemoveActor(id);
    }
    else
    {
      _cacheService.SetActor(actor.ToActor(id));
    }

    DictionaryEntity[] dictionaries = await _context.Dictionaries.Where(x => x.CreatedById == id || x.UpdatedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (DictionaryEntity dictionary in dictionaries)
    {
      dictionary.SetActor(id, actor);
    }

    ExternalIdentifierEntity[] externalIdentifiers = await _context.ExternalIdentifiers.Where(x => x.CreatedById == id || x.UpdatedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (ExternalIdentifierEntity externalIdentifier in externalIdentifiers)
    {
      externalIdentifier.SetActor(id, actor);
    }

    MessageEntity[] messages = await _context.Messages.Where(x => x.CreatedById == id || x.UpdatedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (MessageEntity message in messages)
    {
      message.SetActor(id, actor);
    }

    RealmEntity[] realms = await _context.Realms.Where(x => x.CreatedById == id || x.UpdatedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (RealmEntity realm in realms)
    {
      realm.SetActor(id, actor);
    }

    SenderEntity[] senders = await _context.Senders.Where(x => x.CreatedById == id || x.UpdatedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (SenderEntity sender in senders)
    {
      sender.SetActor(id, actor);
    }

    SessionEntity[] sessions = await _context.Sessions.Where(x => x.CreatedById == id
        || x.UpdatedById == id || x.SignedOutById == id)
      .ToArrayAsync(cancellationToken);
    foreach (SessionEntity session in sessions)
    {
      session.SetActor(id, actor);
    }

    UserEntity[] users = await _context.Users.Where(x => x.CreatedById == id || x.UpdatedById == id
        || x.PasswordChangedById == id || x.DisabledById == id
        || x.AddressVerifiedById == id || x.EmailVerifiedById == id || x.PhoneVerifiedById == id)
      .ToArrayAsync(cancellationToken);
    foreach (UserEntity user in users)
    {
      user.SetActor(id, actor);
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
