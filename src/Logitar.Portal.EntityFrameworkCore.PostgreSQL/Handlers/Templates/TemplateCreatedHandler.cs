using Logitar.Portal.Core.Templates.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Templates;

internal class TemplateCreatedHandler : INotificationHandler<TemplateCreated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public TemplateCreatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(TemplateCreated notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.RealmId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity '{notification.RealmId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    TemplateEntity template = new(notification, realm, actor);

    _context.Templates.Add(template);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
