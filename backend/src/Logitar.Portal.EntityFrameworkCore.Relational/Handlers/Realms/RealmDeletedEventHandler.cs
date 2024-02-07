using Logitar.Identity.EntityFrameworkCore.Relational.CustomAttributes;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Realms;

internal class RealmDeletedEventHandler : INotificationHandler<RealmDeletedEvent>
{
  private const string EntityType = nameof(PortalContext.Realms);

  private readonly PortalContext _context;
  private readonly ICustomAttributeService _customAttributes;

  public RealmDeletedEventHandler(PortalContext context, ICustomAttributeService customAttributes)
  {
    _context = context;
    _customAttributes = customAttributes;
  }

  public async Task Handle(RealmDeletedEvent @event, CancellationToken cancellationToken)
  {
    RealmEntity? realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (realm != null)
    {
      _context.Realms.Remove(realm);

      await _customAttributes.RemoveAsync(EntityType, realm.RealmId, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
