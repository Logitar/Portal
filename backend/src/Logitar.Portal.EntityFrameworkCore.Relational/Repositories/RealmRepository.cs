using Logitar.Data;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class RealmRepository : EventSourcing.EntityFrameworkCore.Relational.AggregateRepository, IRealmRepository
{
  private static readonly string AggregateType = typeof(Realm).GetNamespaceQualifiedName();

  private readonly ISqlHelper _sqlHelper;

  public RealmRepository(IEventBus eventBus, EventContext eventContext, IEventSerializer eventSerializer, ISqlHelper sqlHelper)
    : base(eventBus, eventContext, eventSerializer)
  {
    _sqlHelper = sqlHelper;
  }

  public async Task<IReadOnlyCollection<Realm>> LoadAsync(CancellationToken cancellationToken)
    => (await LoadAsync<Realm>(cancellationToken)).ToArray(); // ISSUE #528: remove ToArray

  public async Task<Realm?> LoadAsync(Guid id, CancellationToken cancellationToken)
    => await LoadAsync<Realm>(new AggregateId(id), cancellationToken);

  public async Task<Realm?> LoadAsync(RealmId id, CancellationToken cancellationToken)
    => await LoadAsync(id, version: null, cancellationToken);
  public async Task<Realm?> LoadAsync(RealmId id, long? version, CancellationToken cancellationToken)
    => await LoadAsync<Realm>(id.AggregateId, version, cancellationToken);

  public async Task<Realm?> LoadAsync(Slug uniqueSlug, CancellationToken cancellationToken)
  {
    IQuery query = _sqlHelper.QueryFrom(EventDb.Events.Table)
      .Join(PortalDb.Realms.AggregateId, EventDb.Events.AggregateId,
        new OperatorCondition(EventDb.Events.AggregateType, Operators.IsEqualTo(AggregateType))
      )
      .Where(PortalDb.Realms.UniqueSlugNormalized, Operators.IsEqualTo(uniqueSlug.Value.ToUpper()))
      .SelectAll(EventDb.Events.Table)
      .Build();

    EventEntity[] events = await EventContext.Events.FromQuery(query)
      .AsNoTracking()
      .OrderBy(e => e.Version)
      .ToArrayAsync(cancellationToken);

    return Load<Realm>(events.Select(EventSerializer.Deserialize)).SingleOrDefault();
  }

  public async Task SaveAsync(Realm realm, CancellationToken cancellationToken)
    => await base.SaveAsync(realm, cancellationToken);
  public async Task SaveAsync(IEnumerable<Realm> realms, CancellationToken cancellationToken)
    => await base.SaveAsync(realms, cancellationToken);
}
