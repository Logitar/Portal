using AutoMapper;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class DictionaryQuerier : IDictionaryQuerier
  {
    private readonly DbSet<DictionaryEntity> _dictionaries;
    private readonly IMapper _mapper;

    public DictionaryQuerier(PortalContext context, IMapper mapper)
    {
      _dictionaries = context.Dictionaries;
      _mapper = mapper;
    }

    public async Task<DictionaryModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);
    public async Task<DictionaryModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return _mapper.Map<DictionaryModel>(dictionary);
    }

    public async Task<ListModel<DictionaryModel>> GetPagedAsync(CultureInfo? locale, string? realm,
      DictionarySort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<DictionaryEntity> query = _dictionaries.AsNoTracking()
        .Include(x => x.Realm);

      query = realm == null
        ? query.Where(x => x.RealmId == null)
        : query.Where(x => x.Realm!.AliasNormalized == realm.ToUpper() || x.Realm.AggregateId == realm);

      if (locale != null)
      {
        query = query.Where(x => x.Locale == locale);
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          DictionarySort.RealmLocale => isDescending ? query.OrderByDescending(x => x.Realm!.DisplayName ?? x.Realm.Alias).ThenByDescending(x => x.Locale) : query.OrderBy(x => x.Realm!.DisplayName ?? x.Realm.Alias).ThenBy(x => x.Locale),
          DictionarySort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The dictionary sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      DictionaryEntity[] dictionaries = await query.ToArrayAsync(cancellationToken);

      return new ListModel<DictionaryModel>(_mapper.Map<IEnumerable<DictionaryModel>>(dictionaries), total);
    }
  }
}
