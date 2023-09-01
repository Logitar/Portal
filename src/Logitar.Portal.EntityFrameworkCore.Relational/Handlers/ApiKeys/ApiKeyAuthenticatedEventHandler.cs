using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class ApiKeyAuthenticatedEventHandler : INotificationHandler<ApiKeyAuthenticatedEvent>
{
  private readonly PortalContext _context;

  public ApiKeyAuthenticatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(ApiKeyAuthenticatedEvent @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity apiKey = await _context.ApiKeys
      .Include(x => x.Roles)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<ApiKeyEntity>(@event.AggregateId);

    apiKey.Authenticate(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
