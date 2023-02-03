using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class SenderQuerier : ISenderQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<SenderEntity> _senders;

    public SenderQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _senders = context.Senders;
    }

    public async Task<SenderModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);
    public async Task<SenderModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      SenderEntity? sender = await _senders.AsNoTracking()
        .Include(x => x.Realm).ThenInclude(x => x!.PasswordRecoverySender)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return await _mapper.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<SenderModel?> GetDefaultAsync(Realm? realm, CancellationToken cancellationToken)
    {
      SenderEntity? sender = await _senders.AsNoTracking()
        .Include(x => x.Realm).ThenInclude(x => x!.PasswordRecoverySender)
        .SingleOrDefaultAsync(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.IsDefault, cancellationToken);

      return await _mapper.MapAsync<SenderModel>(sender, cancellationToken);
    }

    public async Task<ListModel<SenderModel>> GetPagedAsync(ProviderType? provider, string? realm, string? search,
      SenderSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<SenderEntity> query = _senders.AsNoTracking()
        .Include(x => x.Realm).ThenInclude(x => x!.PasswordRecoverySender);

      query = realm == null
        ? query.Where(x => x.RealmId == null)
        : query.Where(x => x.Realm!.AliasNormalized == realm.ToUpper() || x.Realm.AggregateId == realm);

      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.EmailAddress, pattern)
              || (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
          }
        }
      }
      if (provider.HasValue)
      {
        query = query.Where(x => x.Provider == provider.Value);
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

      query = query.ApplyPaging(index, count);

      SenderEntity[] senders = await query.ToArrayAsync(cancellationToken);

      return new ListModel<SenderModel>(await _mapper.MapAsync<SenderModel>(senders, cancellationToken));
    }
  }
}
