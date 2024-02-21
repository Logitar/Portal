using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Messages;

internal class MessageCreatedEventHandler : INotificationHandler<MessageCreatedEvent>
{
  private readonly PortalContext _context;
  private readonly DbSet<UserEntity> _users;

  public MessageCreatedEventHandler(IdentityContext identityContext, PortalContext portalContext)
  {
    _context = portalContext;
    _users = identityContext.Users;
  }

  public async Task Handle(MessageCreatedEvent @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message == null)
    {
      SenderEntity sender = await _context.Senders
        .SingleOrDefaultAsync(x => x.AggregateId == @event.Sender.Id.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The sender entity 'AggregateId={@event.Sender.Id}' could not be found.");
      TemplateEntity template = await _context.Templates
        .SingleOrDefaultAsync(x => x.AggregateId == @event.Template.Id.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The template entity 'AggregateId={@event.Template.Id}' could not be found.");

      int capacity = @event.Recipients.Count;
      HashSet<string> userIds = new(capacity);
      foreach (RecipientUnit recipient in @event.Recipients)
      {
        if (recipient.UserId != null)
        {
          userIds.Add(recipient.UserId.Value);
        }
      }
      Dictionary<string, UserEntity> users = new(capacity);
      if (userIds.Count > 0)
      {
        UserEntity[] userEntities = await _users.Where(u => userIds.Contains(u.AggregateId)).ToArrayAsync(cancellationToken);
        foreach (UserEntity user in userEntities)
        {
          users[user.AggregateId] = user;
        }
      }

      message = new(sender, template, users, @event);

      _context.Messages.Add(message);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
