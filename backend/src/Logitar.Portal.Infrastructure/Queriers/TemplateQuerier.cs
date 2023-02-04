using Logitar.Portal.Application.Templates;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class TemplateQuerier : ITemplateQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<TemplateEntity> _templates;

    public TemplateQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _templates = context.Templates;
    }

    public async Task<TemplateModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);
    public async Task<TemplateModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      TemplateEntity? template = await _templates.AsNoTracking()
        .Include(x => x.Realm).ThenInclude(x => x!.PasswordRecoveryTemplate)
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return await _mapper.MapAsync<TemplateModel>(template, cancellationToken);
    }

    public async Task<ListModel<TemplateModel>> GetPagedAsync(string? realm, string? search,
      TemplateSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<TemplateEntity> query = _templates.AsNoTracking()
        .Include(x => x.Realm).ThenInclude(x => x!.PasswordRecoveryTemplate);

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

            query = query.Where(x => EF.Functions.ILike(x.Key, pattern)
              || (x.DisplayName != null && EF.Functions.ILike(x.DisplayName, pattern)));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          TemplateSort.DisplayName => isDescending ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName),
          TemplateSort.Key => isDescending ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key),
          TemplateSort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The template sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      TemplateEntity[] templates = await query.ToArrayAsync(cancellationToken);

      return new ListModel<TemplateModel>(await _mapper.MapAsync<TemplateModel>(templates, cancellationToken), total);
    }
  }
}
