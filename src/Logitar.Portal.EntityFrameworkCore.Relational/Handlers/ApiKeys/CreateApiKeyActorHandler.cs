using Logitar.Portal.Domain.ApiKeys.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.ApiKeys;

internal class CreateApiKeyActorHandler : INotificationHandler<ApiKeyCreatedEvent>
{
  private readonly PortalContext _context;

  public CreateApiKeyActorHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(ApiKeyCreatedEvent @event, CancellationToken cancellationToken)
  {
    ActorEntity? actor = await _context.Actors.AsNoTracking()
      .SingleOrDefaultAsync(x => x.Id == @event.AggregateId.ToGuid(), cancellationToken);
    if (actor == null)
    {
      actor = new(@event);

      _context.Actors.Add(actor);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
