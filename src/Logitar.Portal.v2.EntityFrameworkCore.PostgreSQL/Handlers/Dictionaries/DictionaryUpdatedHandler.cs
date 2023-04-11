using Logitar.Portal.v2.Core.Dictionaries.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Dictionaries;

internal class DictionaryUpdatedHandler : INotificationHandler<DictionaryUpdated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public DictionaryUpdatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(DictionaryUpdated notification, CancellationToken cancellationToken)
  {
    DictionaryEntity dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The user entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    dictionary.Update(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
