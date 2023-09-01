using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class ApiKeyDeletedEventHandler : INotificationHandler<ApiKeyDeletedEvent>
{
  private readonly PortalContext _context;

  public ApiKeyDeletedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(ApiKeyDeletedEvent @event, CancellationToken cancellationToken)
  {
    ApiKeyEntity? apiKey = await _context.ApiKeys
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (apiKey != null)
    {
      _context.ApiKeys.Remove(apiKey);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
