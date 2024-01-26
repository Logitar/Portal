using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Realms.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class ReplaceRealmCommandHandler : IRequestHandler<ReplaceRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public ReplaceRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(ReplaceRealmCommand command, CancellationToken cancellationToken)
  {
    ReplaceRealmPayload payload = command.Payload;
    new ReplaceRealmValidator().ValidateAndThrow(payload);

    RealmId realmId = new(command.Id);
    RealmAggregate? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    RealmAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _realmRepository.LoadAsync(realmId, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    UniqueSlugUnit uniqueSlug = new(payload.UniqueSlug);
    if (reference == null || uniqueSlug != reference.UniqueSlug)
    {
      realm.SetUniqueSlug(uniqueSlug, actorId);
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      realm.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      realm.Description = description;
    }

    LocaleUnit? defaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale);
    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      realm.DefaultLocale = defaultLocale;
    }
    JwtSecretUnit? secret = JwtSecretUnit.CreateOrGenerate(payload.Secret);
    if (reference == null || secret != reference.Secret)
    {
      realm.Secret = secret;
    }
    UrlUnit? url = UrlUnit.TryCreate(payload.Url);
    if (reference == null || url != reference.Url)
    {
      realm.Url = url;
    }

    ReadOnlyUniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      realm.UniqueNameSettings = uniqueNameSettings;
    }
    ReadOnlyPasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      realm.PasswordSettings = passwordSettings;
    }
    if (reference == null || payload.RequireUniqueEmail != reference.RequireUniqueEmail)
    {
      realm.RequireUniqueEmail = payload.RequireUniqueEmail;
    }

    foreach (CustomAttribute customAttribute in payload.CustomAttributes)
    {
      // TODO(fpion): implement
    }

    realm.Update(actorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.ReadAsync(realm, cancellationToken);
  }
}
