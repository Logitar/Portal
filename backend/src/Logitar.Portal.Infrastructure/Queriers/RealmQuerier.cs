using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Queriers
{
  internal class RealmQuerier : IRealmQuerier
  {
    private readonly IMappingService _mapper;
    private readonly DbSet<RealmEntity> _realms;

    public RealmQuerier(PortalContext context, IMappingService mapper)
    {
      _mapper = mapper;
      _realms = context.Realms;
    }

    public async Task<RealmModel?> GetAsync(string aggregateIdOrAlias, CancellationToken cancellationToken = default)
    {
      RealmEntity? realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == aggregateIdOrAlias || x.AliasNormalized == aggregateIdOrAlias.ToUpper(), cancellationToken);

      return await _mapper.MapAsync<RealmModel>(realm, cancellationToken);
    }
  }
}
