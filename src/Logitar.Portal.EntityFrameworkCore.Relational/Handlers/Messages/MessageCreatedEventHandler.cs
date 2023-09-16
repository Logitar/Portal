using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Messages;

internal class MessageCreatedEventHandler : INotificationHandler<MessageCreatedEvent>
{
  private readonly PortalContext _context;

  public MessageCreatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(MessageCreatedEvent @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message == null)
    {
      RealmEntity? realm = null;
      if (@event.Realm != null)
      {
        realm = await _context.Realms
          .SingleOrDefaultAsync(x => x.AggregateId == @event.Realm.Id.Value, cancellationToken)
          ?? throw new EntityNotFoundException<RealmEntity>(@event.Realm.Id);
      }

      SenderEntity sender = await _context.Senders
        .SingleOrDefaultAsync(x => x.AggregateId == @event.Sender.Id.Value, cancellationToken)
        ?? throw new EntityNotFoundException<SenderEntity>(@event.Sender.Id);

      TemplateEntity template = await _context.Templates
        .SingleOrDefaultAsync(x => x.AggregateId == @event.Template.Id.Value, cancellationToken)
        ?? throw new EntityNotFoundException<TemplateEntity>(@event.Template.Id);

      IEnumerable<string> userIds = @event.Recipients.Where(x => x.User != null).Select(x => x.User!.Id.Value).Distinct();
      UserEntity[] users = await _context.Users.Where(x => userIds.Contains(x.AggregateId)).ToArrayAsync(cancellationToken);

      message = new(@event, realm, sender, template, users);

      _context.Messages.Add(message);
      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
