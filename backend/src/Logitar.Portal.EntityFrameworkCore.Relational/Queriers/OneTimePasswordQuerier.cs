using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Application;
using Logitar.Portal.Application.OneTimePasswords;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Queriers;

internal class OneTimePasswordQuerier : IOneTimePasswordQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<OneTimePasswordEntity> _oneTimePasswords;
  private readonly ISearchHelper _searchHelper;
  private readonly ISqlHelper _sqlHelper;

  public OneTimePasswordQuerier(IActorService actorService, IApplicationContext applicationContext,
    IdentityContext context, ISearchHelper searchHelper, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _oneTimePasswords = context.OneTimePasswords;
    _searchHelper = searchHelper;
    _sqlHelper = sqlHelper;
  }

  public async Task<OneTimePassword> ReadAsync(OneTimePasswordAggregate oneTimePassword, CancellationToken cancellationToken)
  {
    return await ReadAsync(oneTimePassword.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The One-Time Password entity 'AggregateId={oneTimePassword.Id.Value}' could not be found.");
  }
  public async Task<OneTimePassword?> ReadAsync(OneTimePasswordId id, CancellationToken cancellationToken)
    => await ReadAsync(id.AggregateId.ToGuid(), cancellationToken);
  public async Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    string aggregateId = new AggregateId(id).Value;

    OneTimePasswordEntity? oneTimePassword = await _oneTimePasswords.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == aggregateId, cancellationToken);

    Realm? realm = _applicationContext.Realm;
    if (oneTimePassword == null || oneTimePassword.TenantId != _applicationContext.TenantId?.Value)
    {
      return null;
    }

    return await MapAsync(oneTimePassword, realm, cancellationToken);
  }

  private async Task<OneTimePassword> MapAsync(OneTimePasswordEntity oneTimePassword, Realm? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([oneTimePassword], realm, cancellationToken)).Single();
  private async Task<IEnumerable<OneTimePassword>> MapAsync(IEnumerable<OneTimePasswordEntity> oneTimePasswords, Realm? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = oneTimePasswords.SelectMany(oneTimePassword => oneTimePassword.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return oneTimePasswords.Select(oneTimePassword => mapper.ToOneTimePassword(oneTimePassword, realm));
  }
}
