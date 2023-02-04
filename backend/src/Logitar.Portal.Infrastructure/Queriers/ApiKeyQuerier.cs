using AutoMapper;
using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Domain;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class ApiKeyQuerier : IApiKeyQuerier
  {
    private readonly DbSet<ApiKeyEntity> _apiKeys;
    private readonly IMapper _mapper;

    public ApiKeyQuerier(PortalContext context, IMapper mapper)
    {
      _apiKeys = context.ApiKeys;
      _mapper = mapper;
    }

    public async Task<ApiKeyModel?> GetAsync(AggregateId id, CancellationToken cancellationToken)
      => await GetAsync(id.Value, cancellationToken);
    public async Task<ApiKeyModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      ApiKeyEntity? apiKey = await _apiKeys.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

      return _mapper.Map<ApiKeyModel>(apiKey);
    }

    public async Task<ListModel<ApiKeyModel>> GetPagedAsync(DateTime? expiredOn, string? search,
      ApiKeySort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      IQueryable<ApiKeyEntity> query = _apiKeys.AsNoTracking();

      if (expiredOn.HasValue)
      {
        query = query.Where(x => x.ExpiresOn <= expiredOn.Value);
      }
      if (search != null)
      {
        foreach (string term in search.Split())
        {
          if (!string.IsNullOrEmpty(term))
          {
            string pattern = $"%{term}%";

            query = query.Where(x => EF.Functions.ILike(x.Title, pattern));
          }
        }
      }

      long total = await query.LongCountAsync(cancellationToken);

      if (sort.HasValue)
      {
        query = sort.Value switch
        {
          ApiKeySort.ExpiresOn => isDescending ? query.OrderByDescending(x => x.ExpiresOn) : query.OrderBy(x => x.ExpiresOn),
          ApiKeySort.Title => isDescending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
          ApiKeySort.UpdatedOn => isDescending ? query.OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn) : query.OrderBy(x => x.UpdatedOn ?? x.CreatedOn),
          _ => throw new ArgumentException($"The API key sort '{sort}' is not valid.", nameof(sort)),
        };
      }

      query = query.ApplyPaging(index, count);

      ApiKeyEntity[] apiKeys = await query.ToArrayAsync(cancellationToken);

      return new ListModel<ApiKeyModel>(_mapper.Map<IEnumerable<ApiKeyModel>>(apiKeys), total);
    }
  }
}
