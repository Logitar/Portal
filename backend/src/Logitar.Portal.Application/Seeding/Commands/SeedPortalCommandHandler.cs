using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Seeding.Commands;

internal class SeedPortalCommandHandler : INotificationHandler<SeedPortalCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IRealmRepository _realmRepository;
  private readonly SeedingSettings _settings;
  private readonly IUserRepository _userRepository;

  public SeedPortalCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager,
    IRealmRepository realmRepository, SeedingSettings settings, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _realmRepository = realmRepository;
    _settings = settings;
    _userRepository = userRepository;
  }

  public async Task Handle(SeedPortalCommand _, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await _realmRepository.LoadAsync(RealmAggregate.AdministrationUniqueSlug, cancellationToken);
    if (realm == null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      realm = new(RealmAggregate.AdministrationUniqueSlug, actorId)
      {
        DisplayName = new DisplayNameUnit(_settings.Realm.DisplayName),
        Description = new DescriptionUnit(_settings.Realm.Description)
      };
      realm.Update(actorId);
      _applicationContext.Realm = ToModel(realm);

      UniqueNameUnit uniqueName = new(realm.UniqueNameSettings, _settings.User.UniqueName);
      TenantId tenantId = new(realm.Id.Value);
      UserAggregate user = new(uniqueName, tenantId, actorId, userId);

      Password password = _passwordManager.Create(_settings.User.Password);
      user.SetPassword(password, actorId);

      await _realmRepository.SaveAsync(realm, cancellationToken);
      await _userRepository.SaveAsync(user, cancellationToken);
    }
  }

  private static Realm ToModel(RealmAggregate realm)
  {
    Realm model = new(realm.UniqueSlug.Value, realm.Secret.Value)
    {
      Id = realm.Id.Value,
      Version = realm.Version,
      CreatedBy = new Actor
      {
        Id = realm.CreatedBy.Value
      },
      CreatedOn = realm.CreatedOn,
      UpdatedBy = new Actor
      {
        Id = realm.UpdatedBy.Value
      },
      UpdatedOn = realm.UpdatedOn,
      DisplayName = realm.DisplayName?.Value,
      Description = realm.Description?.Value,
      DefaultLocale = realm.DefaultLocale?.Code,
      Url = realm.Url?.Value,
      UniqueNameSettings = new UniqueNameSettings
      {
        AllowedCharacters = realm.UniqueNameSettings.AllowedCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = realm.PasswordSettings.RequiredLength,
        RequiredUniqueChars = realm.PasswordSettings.RequiredUniqueChars,
        RequireNonAlphanumeric = realm.PasswordSettings.RequireNonAlphanumeric,
        RequireLowercase = realm.PasswordSettings.RequireLowercase,
        RequireUppercase = realm.PasswordSettings.RequireUppercase,
        RequireDigit = realm.PasswordSettings.RequireDigit,
        HashingStrategy = realm.PasswordSettings.HashingStrategy
      },
      RequireUniqueEmail = realm.RequireUniqueEmail
    };

    foreach (KeyValuePair<string, string> customAttribute in realm.CustomAttributes)
    {
      model.CustomAttributes.Add(new CustomAttribute(customAttribute));
    }

    return model;
  }
}
