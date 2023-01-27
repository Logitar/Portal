using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class SessionQuerier : ISessionQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<SessionEntity> _sessions;

    public SessionQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _sessions = context.Sessions;
    }

    public async Task<SessionModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      SessionEntity? session = await _sessions.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return session == null ? null : await _mapper.MapAsync<SessionModel>(session, cancellationToken);
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
        : query.Where(x => x.User!.Realm!.AggregateId == realm || x.User.Realm.AliasNormalized == realm.ToUpper());

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
        query = query.Where(x => x.User!.AggregateId == userId);
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
