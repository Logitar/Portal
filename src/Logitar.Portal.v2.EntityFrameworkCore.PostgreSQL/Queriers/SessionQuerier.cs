using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Core.Sessions;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _sessions = context.Sessions;
  }

  public async Task<Session> GetAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    SessionEntity entity = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.ExternalIdentifiers)
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == session.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session entity '{session.Id}' could not be found.");

    return _mapper.Map<Session>(entity);
  }

  public async Task<Session?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.ExternalIdentifiers)
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Session>(session);
  }

  public async Task<IEnumerable<Session>> GetAsync(IEnumerable<SessionAggregate> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<string> aggregateIds = sessions.Select(session => session.Id.Value).Distinct();

    SessionEntity[] entities = await _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.ExternalIdentifiers)
      .Include(x => x.User).ThenInclude(x => x!.Realm)
      .Where(x => aggregateIds.Contains(x.AggregateId))
      .ToArrayAsync(cancellationToken);

    return _mapper.Map<IEnumerable<Session>>(entities);
  }

  public async Task<PagedList<Session>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
    SessionSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<SessionEntity> query = _sessions.AsNoTracking()
      .Include(x => x.User).ThenInclude(x => x!.ExternalIdentifiers)
      .Include(x => x.User).ThenInclude(x => x!.Realm);

    if (isActive.HasValue)
    {
      query = query.Where(x => x.IsActive == isActive.Value);
    }
    if (isPersistent.HasValue)
    {
      query = query.Where(x => x.IsPersistent == isPersistent.Value);
    }
    if (realm != null)
    {
      string aggregateId = Guid.TryParse(realm, out Guid realmId)
        ? new AggregateId(realmId).Value
        : realm;

      query = query.Where(x => x.User!.Realm!.AggregateId == aggregateId || x.User!.Realm.UniqueNameNormalized == realm.ToUpper());
    }
    if (userId.HasValue)
    {
      string aggregateId = new AggregateId(userId.Value).Value;

      query = query.Where(x => x.User!.AggregateId == aggregateId);
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        SessionSort.SignedOutOn => isDescending ? query.OrderByDescending(x => x.SignedOutOn) : query.OrderBy(x => x.SignedOutOn),
        SessionSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        SessionSort.User => isDescending ? query.OrderByDescending(x => x.User!.FullName ?? x.User.Username) : query.OrderBy(x => x.User!.FullName ?? x.User.Username),
        _ => throw new ArgumentException($"The session sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    SessionEntity[] sessions = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Session>
    {
      Items = _mapper.Map<IEnumerable<Session>>(sessions),
      Total = total
    };
  }
}
