using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class SenderRepository : Repository, ISenderRepository
{
  private readonly DbSet<SenderEntity> _senders;

  public SenderRepository(PortalContext context, IEventStore eventStore) : base(eventStore)
  {
    _senders = context.Senders;
  }

  public async Task<Sender?> LoadAsync(SenderId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public async Task<Sender?> LoadAsync(SenderId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Sender>(id.StreamId, version, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Sender>> LoadAsync(CancellationToken cancellationToken)
  {
    return await LoadAsync<Sender>(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Sender>> LoadAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    Guid? tenantIdValue = tenantId?.ToGuid();

    IEnumerable<StreamId> streamIds = (await _senders.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken)).Select(value => new StreamId(value));

    return await LoadAsync<Sender>(streamIds, cancellationToken);
  }

  public async Task<Sender?> LoadDefaultAsync(TenantId? tenantId, CancellationToken cancellationToken)
  {
    Guid? tenantIdValue = tenantId?.ToGuid();

    string? streamId = await _senders.AsNoTracking()
      .Where(x => x.TenantId == tenantIdValue && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (streamId == null)
    {
      return null;
    }

    return await LoadAsync<Sender>(new StreamId(streamId), cancellationToken);
  }

  public async Task SaveAsync(Sender sender, CancellationToken cancellationToken)
  {
    await base.SaveAsync(sender, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Sender> senders, CancellationToken cancellationToken)
  {
    await base.SaveAsync(senders, cancellationToken);
  }
}
