using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class MessageEvents : INotificationHandler<MessageCreated>,
  INotificationHandler<MessageDeleted>,
  INotificationHandler<MessageFailed>,
  INotificationHandler<MessageSucceeded>
{
  private readonly PortalContext _context;
  private readonly DbSet<UserEntity> _users;

  public MessageEvents(PortalContext portalContext, IdentityContext identityContext)
  {
    _context = portalContext;
    _users = identityContext.Users;
  }

  public async Task Handle(MessageCreated @event, CancellationToken cancellationToken)
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
      foreach (Recipient recipient in @event.Recipients)
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

  public async Task Handle(MessageDeleted @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message != null)
    {
      _context.Messages.Remove(message);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(MessageFailed @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message != null)
    {
      message.Fail(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(MessageSucceeded @event, CancellationToken cancellationToken)
  {
    MessageEntity? message = await _context.Messages
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (message != null)
    {
      message.Succeed(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
