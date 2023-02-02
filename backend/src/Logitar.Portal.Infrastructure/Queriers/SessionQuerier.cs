using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class SessionQuerier : ISessionQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<SessionEntity> _sessions;

    public SessionQuerier(IMappingService mapper, PortalContext context)
    {
      _mapper = mapper;
      _sessions = context.Sessions;
    }

    public async Task<SessionModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);

    public async Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      SessionEntity? session = await _sessions.AsNoTracking()
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return await _mapper.MapAsync<SessionModel>(session, cancellationToken);
    }

    public async Task<IEnumerable<SessionModel>> GetAsync(IEnumerable<AggregateId> ids, CancellationToken cancellationToken)
    {
      IEnumerable<string> aggregateIds = ids.Select(x => x.Value);

      SessionEntity[] sessions = await _sessions.AsNoTracking()
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .Where(x => aggregateIds.Contains(x.AggregateId))
        .ToArrayAsync(cancellationToken);

      return await _mapper.MapAsync<SessionModel>(sessions, cancellationToken);
    }

    public async Task<ListModel<SessionModel>> GetPagedAsync(bool? isActive, bool? isPersistent, string? realm, string? userId,
      SessionSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<SessionEntity> query = _sessions.AsNoTracking()
        .Include(x => x.User).ThenInclude(x => x!.Realm);

      query = realm == null
        ? query.Where(x => x.User!.RealmId == null)
        : query.Where(x => x.User!.Realm!.AliasNormalized == realm.ToUpper() || x.User.Realm.AggregateId == realm);

      if (isActive.HasValue)
      {
        query = query.Where(x => x.IsActive == isActive.Value);
      }
      if (isPersistent.HasValue)
      {
        query = query.Where(x => x.IsPersistent == isPersistent.Value);
      }
      if (userId != null)
      {
        query = query.Where(x => x.User != null && x.User.AggregateId == userId);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          SessionSort.IpAddress => isDescending ? query.OrderByDescending(x => x.IpAddress) : query.OrderBy(x => x.IpAddress),
          SessionSort.SignedOutOn => isDescending ? query.OrderByDescending(x => x.SignedOutOn) : query.OrderBy(x => x.SignedOutOn),
          SessionSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The session sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      SessionEntity[] sessions = await query.ToArrayAsync(cancellationToken);

      return new ListModel<SessionModel>(await _mapper.MapAsync<SessionModel>(sessions, cancellationToken), total);
    }
  }
}
