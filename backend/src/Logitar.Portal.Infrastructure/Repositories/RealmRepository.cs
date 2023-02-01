using Logitar.Portal.Application.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Repositories
{
  internal class RealmRepository : Repository, IRealmRepository
  {
    private readonly DbSet<RealmEntity> _realms;

    public RealmRepository(PortalContext context, IPublisher publisher) : base(context, publisher)
    {
      _realms = context.Realms;
    }

    public async Task<Realm?> LoadByAliasOrIdAsync(string aliasOrId, CancellationToken cancellationToken)
    {
      RealmEntity? realm = await _realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AliasNormalized == aliasOrId.ToUpper()
          || x.AggregateId == aliasOrId, cancellationToken);

      return realm == null ? null : await LoadAsync<Realm>(realm.AggregateId, cancellationToken);
    }
  }
}
