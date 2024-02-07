using Logitar.Identity.EntityFrameworkCore.Relational.CustomAttributes;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmUpdatedEventHandler : INotificationHandler<RealmUpdatedEvent>
{
  private const string EntityType = nameof(PortalContext.Realms);

  private readonly PortalContext _context;
  private readonly ICustomAttributeService _customAttributes;

  public RealmUpdatedEventHandler(PortalContext context, ICustomAttributeService customAttributes)
  {
    _context = context;
    _customAttributes = customAttributes;
  }

  public async Task Handle(RealmUpdatedEvent @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (realm != null)
    {
      realm.Update(@event);

      await _customAttributes.SynchronizeAsync(EntityType, realm.RealmId, @event.CustomAttributes, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
