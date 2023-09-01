using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class ApiKeyCreatedEventHandler : INotificationHandler<ApiKeyCreatedEvent>
{
  private readonly PortalContext _context;

  public ApiKeyCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(ApiKeyCreatedEvent @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await _context.ApiKeys.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (apiKey == null)
    {
      apiKey = new(@event);

      _context.ApiKeys.Add(apiKey);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
