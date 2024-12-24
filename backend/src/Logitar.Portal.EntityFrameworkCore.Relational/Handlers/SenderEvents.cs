using Logitar.Portal.Domain.Senders.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class SenderEvents : INotificationHandler<EmailSenderCreated>,
  INotificationHandler<SenderDeleted>,
  INotificationHandler<SenderMailgunSettingsChanged>,
  INotificationHandler<SenderSendGridSettingsChanged>,
  INotificationHandler<SenderSetDefault>,
  INotificationHandler<SenderTwilioSettingsChanged>,
  INotificationHandler<SenderUpdated>,
  INotificationHandler<SmsSenderCreated>
{
  private readonly PortalContext _context;

  public SenderEvents(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(EmailSenderCreated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender == null)
    {
      sender = new(@event);

      _context.Senders.Add(sender);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(SenderDeleted @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender != null)
    {
      _context.Senders.Remove(sender);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(SenderMailgunSettingsChanged @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.StreamId}' could not be found.");

    sender.SetMailgunSettings(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(SenderSendGridSettingsChanged @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.StreamId}' could not be found.");

    sender.SetSendGridSettings(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(SenderSetDefault @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.StreamId}' could not be found.");

    sender.SetDefault(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(SenderTwilioSettingsChanged @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.StreamId}' could not be found.");

    sender.SetTwilioSettings(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(SenderUpdated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity 'StreamId={@event.StreamId}' could not be found.");

    sender.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task Handle(SmsSenderCreated @event, CancellationToken cancellationToken)
  {
    SenderEntity? sender = await _context.Senders.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (sender == null)
    {
      sender = new(@event);

      _context.Senders.Add(sender);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
