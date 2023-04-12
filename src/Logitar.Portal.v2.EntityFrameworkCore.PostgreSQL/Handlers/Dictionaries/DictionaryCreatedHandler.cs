using Logitar.Portal.v2.Core.Dictionaries.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Dictionarys;

internal class DictionaryCreatedHandler : INotificationHandler<DictionaryCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public DictionaryCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(DictionaryCreated notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.RealmId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    DictionaryEntity dictionary = new(notification, realm, actor);

    _context.Dictionaries.Add(dictionary);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
