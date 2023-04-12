using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Templates;
using Logitar.Portal.v2.Core.Templates;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Queriers;

internal class TemplateQuerier : ITemplateQuerier
{
  private readonly IMapper _mapper;
  private readonly DbSet<TemplateEntity> _templates;

  public TemplateQuerier(PortalContext context, IMapper mapper)
  {
    _mapper = mapper;
    _templates = context.Templates;
  }

  public async Task<Template> GetAsync(TemplateAggregate template, CancellationToken cancellationToken)
  {
    TemplateEntity entity = await _templates.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == template.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The template entity '{template.Id}' could not be found.");

    return _mapper.Map<Template>(entity);
  }

  public async Task<Template?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    TemplateEntity? template = await _templates.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    return _mapper.Map<Template>(template);
  }

  public async Task<Template?> GetAsync(string realm, string key, CancellationToken cancellationToken)
  {
    string aggregateId = (Guid.TryParse(realm, out Guid realmId)
      ? new AggregateId(realmId)
      : new(realm)).Value;

    TemplateEntity? template = await _templates.AsNoTracking()
      .Include(x => x.Realm)
      .SingleOrDefaultAsync(x => x.Realm!.AggregateId == aggregateId
        && x.UniqueNameNormalized == key.ToUpper(), cancellationToken);

    return _mapper.Map<Template>(template);
  }

  public async Task<PagedList<Template>> GetAsync(string? realm, string? search,
    TemplateSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    IQueryable<TemplateEntity> query = _templates.AsNoTracking()
     .Include(x => x.Realm);

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

        query = query.Where(x => EF.Functions.ILike(x.UniqueName, pattern)
          || EF.Functions.ILike(x.DisplayName!, pattern));
      }
    }

    long total = await query.LongCountAsync(cancellationToken);

    if (sort.HasValue)
    {
      query = sort.Value switch
      {
        TemplateSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
        TemplateSort.Key => isDescending ? query.OrderByDescending(x => x.UniqueName) : query.OrderBy(x => x.UniqueName),
        TemplateSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
        _ => throw new ArgumentException($"The template sort '{sort}' is not valid.", nameof(sort)),
      };
    }

    query = query.Page(skip, limit);

    TemplateEntity[] templates = await query.ToArrayAsync(cancellationToken);

    return new PagedList<Template>
    {
      Items = _mapper.Map<IEnumerable<Template>>(templates),
      Total = total
    };
  }
}
