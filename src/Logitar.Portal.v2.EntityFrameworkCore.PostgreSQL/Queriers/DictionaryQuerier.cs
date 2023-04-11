using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using Logitar.Portal.v2.Core.Dictionaries;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class DictionaryQuerier : IDictionaryQuerier
{
  private readonly DbSet<DictionaryEntity> _dictionaries;
  private readonly IMapper _mapper;

  public DictionaryQuerier(PortalContext context, IMapper mapper)
  {
    _dictionaries = context.Dictionaries;
    _mapper = mapper;
  }

  public async Task<Dictionary> GetAsync(DictionaryAggregate dictionary, CancellationToken cancellationToken)
  {
    DictionaryEntity entity = await _dictionaries.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == dictionary.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The dictionary entity '{dictionary.Id}' could not be found.");

    return _mapper.Map<Dictionary>(entity);
  }

  public async Task<Dictionary?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    DictionaryEntity? dictionary = await _dictionaries.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Dictionary>(dictionary);
  }

  public async Task<PagedList<Dictionary>> GetAsync(string? locale, string? realm, DictionarySort? sort,
    bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<DictionaryEntity> query = _dictionaries.AsNoTracking()
      .Include(x => x.Realm);

    if (locale != null)
    {
      query = query.Where(x => x.Locale == locale);
    }
    if (realm != null)
    {

    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        DictionarySort.Entries => isDescending ? query.OrderByDescending(x => x.EntryCount) : query.OrderBy(x => x.EntryCount),
        DictionarySort.RealmLocale => isDescending
          ? query.OrderByDescending(x => x.Realm!.DisplayName ?? x.Realm.UniqueName).ThenByDescending(x => x.Locale)
          : query.OrderBy(x => x.Realm!.DisplayName ?? x.Realm.UniqueName).ThenBy(x => x.Locale),
        DictionarySort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        _ => throw new ArgumentException($"The dictionary sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    DictionaryEntity[] dictionaries = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Dictionary>
    {
      Items = _mapper.Map<IEnumerable<Dictionary>>(dictionaries),
      Total = total
    };
  }
}
