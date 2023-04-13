using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Queriers;

internal class SenderQuerier : ISenderQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<SenderEntity> _senders;

  public SenderQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _senders = context.Senders;
  }

  public async Task<Sender> GetAsync(SenderAggregate sender, CancellationToken cancellationToken)
  {
    SenderEntity entity = await _senders.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == sender.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The sender entity '{sender.Id}' could not be found.");

    return _mapper.Map<Sender>(entity);
  }

  public async Task<Sender?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Sender>(sender);
  }

  public async Task<PagedList<Sender>> GetAsync(ProviderType? provider, string? realm, string? search,
    SenderSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<SenderEntity> query = _senders.AsNoTracking()
      .Include(x => x.Realm);

    if (provider.HasValue)
    {
      query = query.Where(x => x.Provider == provider.Value.ToString());
    }
    if (realm != null)
    {
      string aggregateId = Guid.TryParse(realm, out Guid realmId)
        ? new AggregateId(realmId).Value
        : realm;

      query = query.Where(x => x.Realm!.AggregateId == aggregateId || x.Realm.UniqueNameNormalized == realm.ToUpper());
    }
    if (search != null)
    {
      foreach (string term in search.Split().Where(x => !string.IsNullOrEmpty(x)))
      {
        string pattern = $"%{term}%";

        query = query.Where(x => EF.Functions.ILike(x.EmailAddress, pattern)
          || EF.Functions.ILike(x.DisplayName!, pattern));
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        SenderSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
        SenderSort.EmailAddress => isDescending ? query.OrderByDescending(x => x.EmailAddress) : query.OrderBy(x => x.EmailAddress),
        SenderSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        _ => throw new ArgumentException($"The sender sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    SenderEntity[] senders = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Sender>
    {
      Items = _mapper.Map<IEnumerable<Sender>>(senders),
      Total = total
    };
  }

  public async Task<Sender?> GetDefaultAsync(string realm, CancellationToken cancellationToken)
  {
    string aggregateId = (Guid.TryParse(realm, out Guid realmId)
      ? new AggregateId(realmId)
      : new(realm)).Value;

    SenderEntity? sender = await _senders.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.Realm!.AggregateId == aggregateId
        || x.Realm.UniqueNameNormalized == realm.ToUpper(), cancellationToken);

    return _mapper.Map<Sender>(sender);
  }
}
