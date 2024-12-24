using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
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
  private readonly DbSet<OneTimePasswordEntity> _oneTimePasswords;

  public OneTimePasswordQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _oneTimePasswords = context.OneTimePasswords;
  }

  public async Task<OneTimePasswordModel> ReadAsync(RealmModel? realm, OneTimePassword oneTimePassword, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, oneTimePassword.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The One-Time Password (OTP) entity 'StreamId={oneTimePassword.Id.Value}' could not be found.");
  }
  public async Task<OneTimePasswordModel?> ReadAsync(RealmModel? realm, Guid id, CancellationToken cancellationToken)
  {
    return await ReadAsync(realm, new OneTimePasswordId(realm?.GetTenantId(), new EntityId(id)), cancellationToken);
  }
  public async Task<OneTimePasswordModel?> ReadAsync(RealmModel? realm, OneTimePasswordId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    OneTimePasswordEntity? oneTimePassword = await _oneTimePasswords.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    if (oneTimePassword == null || oneTimePassword.TenantId != realm?.GetTenantId().Value)
    {
      return null;
    }

    return await MapAsync(oneTimePassword, realm, cancellationToken);
  }

  private async Task<OneTimePasswordModel> MapAsync(OneTimePasswordEntity oneTimePassword, RealmModel? realm, CancellationToken cancellationToken = default)
    => (await MapAsync([oneTimePassword], realm, cancellationToken)).Single();
  private async Task<IEnumerable<OneTimePasswordModel>> MapAsync(IEnumerable<OneTimePasswordEntity> oneTimePasswords, RealmModel? realm, CancellationToken cancellationToken = default)
  {
    IEnumerable<ActorId> actorIds = oneTimePasswords.SelectMany(oneTimePassword => oneTimePassword.GetActorIds());
    IEnumerable<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return oneTimePasswords.Select(oneTimePassword => mapper.ToOneTimePassword(oneTimePassword, realm));
  }
}
