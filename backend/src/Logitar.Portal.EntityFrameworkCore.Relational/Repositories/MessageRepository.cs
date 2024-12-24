using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class MessageRepository : Repository, IMessageRepository
{
  private readonly DbSet<MessageEntity> _messages;

  public MessageRepository(PortalContext context, IEventStore eventStore) : base(eventStore)
  {
    _messages = context.Messages;
  }

  public async Task<Message?> LoadAsync(MessageId id, CancellationToken cancellationToken)
  {
    return await LoadAsync<Message>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Message>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Message>(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Message>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    Guid? tenantIdValue = tenantId?.ToGuid();

    IEnumerable<StreamId> streamIds = (await _messages.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken)).Select(value => new StreamId(value));

    return await LoadAsync<Message>(streamIds, cancellationToken);
  }

  public async Task SaveAsync(Message message, CancellationToken cancellationToken)
  {
    await base.SaveAsync(message, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(messages, cancellationToken);
  }
}
