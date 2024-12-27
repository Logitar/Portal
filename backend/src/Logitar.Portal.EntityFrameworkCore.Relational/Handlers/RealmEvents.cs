using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class RealmEvents : INotificationHandler<RealmCreated>,
  INotificationHandler<RealmDeleted>,
  INotificationHandler<RealmUniqueSlugChanged>,
  INotificationHandler<RealmUpdated>
{
  private const string EntityType = "Realm";

  private readonly PortalContext _context;
  private readonly ICustomAttributeService _customAttributes;

  public RealmEvents(PortalContext context, ICustomAttributeService customAttributes)
  {
    _context = context;
    _customAttributes = customAttributes;
  }

  public async Task Handle(RealmCreated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm == null)
    {
      realm = new(@event);

      _context.Realms.Add(realm);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(RealmDeleted @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm != null)
    {
      _context.Realms.Remove(realm);

      await _customAttributes.RemoveAsync(EntityType, realm.RealmId, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(RealmUniqueSlugChanged @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm != null)
    {
      realm.SetUniqueSlug(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(RealmUpdated @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (realm != null)
    {
      realm.Update(@event);

      await _customAttributes.UpdateAsync(EntityType, realm.RealmId, @event.CustomAttributes, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
