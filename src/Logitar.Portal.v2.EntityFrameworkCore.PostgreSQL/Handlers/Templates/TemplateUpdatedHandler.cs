using Logitar.Portal.v2.Core.Templates.Events;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Handlers.Templates;

internal class TemplateUpdatedHandler : INotificationHandler<TemplateUpdated>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public TemplateUpdatedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(TemplateUpdated notification, CancellationToken cancellationToken)
  {
    TemplateEntity template = await _context.Templates
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity '{notification.AggregateId}' could not be found.");

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    template.Update(notification, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
