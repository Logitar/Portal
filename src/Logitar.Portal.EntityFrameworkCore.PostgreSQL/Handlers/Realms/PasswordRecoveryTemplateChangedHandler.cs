using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Actors;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Handlers.Realms;

internal class PasswordRecoveryTemplateChangedHandler : INotificationHandler<PasswordRecoveryTemplateChanged>
{
  private readonly IActorService _actorService;
  private readonly PortalContext _context;

  public PasswordRecoveryTemplateChangedHandler(IActorService actorService, PortalContext context)
  {
    _actorService = actorService;
    _context = context;
  }

  public async Task Handle(PasswordRecoveryTemplateChanged notification, CancellationToken cancellationToken)
  {
    RealmEntity realm = await _context.Realms
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The realm entity '{notification.AggregateId}' could not be found.");

    TemplateEntity? template = null;
    if (notification.TemplateId.HasValue)
    {
      template = await _context.Templates.SingleOrDefaultAsync(x => x.AggregateId == notification.TemplateId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The template entity '{notification.TemplateId}' could not be found.");
    }

    ActorEntity actor = await _actorService.GetAsync(notification, cancellationToken);
    realm.SetPasswordRecoveryTemplate(notification, template, actor);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
