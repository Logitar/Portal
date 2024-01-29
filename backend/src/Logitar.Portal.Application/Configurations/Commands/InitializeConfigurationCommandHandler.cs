using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations.Validators;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand, Session>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(ICacheService cacheService, IConfigurationRepository configurationRepository,
    IPasswordManager passwordManager, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserManager userManager)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userManager = userManager;
  }

  public async Task<Session> Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    InitializeConfigurationPayload payload = command.Payload;
    new InitializeConfigurationValidator().ValidateAndThrow(payload);

    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    UserId userId = UserId.NewId();
    ActorId actorId = new(userId.Value);

    LocaleUnit? locale = LocaleUnit.TryCreate(payload.DefaultLocale, nameof(payload.DefaultLocale));
    configuration = ConfigurationAggregate.Initialize(locale, actorId);

    UserPayload userPayload = payload.User;
    UserSettings userSettings = new()
    {
      UniqueName = configuration.UniqueNameSettings,
      Password = configuration.PasswordSettings,
      RequireUniqueEmail = configuration.RequireUniqueEmail
    };
    new UserPayloadValidator(userSettings).ValidateAndThrow(userPayload);

    UniqueNameUnit uniqueName = new(configuration.UniqueNameSettings, userPayload.UniqueName);
    UserAggregate user = new(uniqueName, tenantId: null, actorId, userId)
    {
      FirstName = PersonNameUnit.TryCreate(userPayload.FirstName),
      LastName = PersonNameUnit.TryCreate(userPayload.LastName),
      Locale = locale
    };
    user.Update(actorId);

    _cacheService.Configuration = BuildConfiguration(configuration, user);
    try
    {
      Password password = _passwordManager.ValidateAndCreate(userPayload.Password);
      user.SetPassword(password, actorId);

      EmailUnit? email = userPayload.Email?.ToEmailUnit();
      user.SetEmail(email, actorId);

      SessionAggregate session = user.SignIn();
      foreach (CustomAttribute customAttribute in payload.Session.CustomAttributes)
      {
        session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
      }
      session.Update(actorId);

      await _userManager.SaveAsync(user, actorId, cancellationToken);
      await _sessionRepository.SaveAsync(session, cancellationToken);
      await _configurationRepository.SaveAsync(configuration, cancellationToken);

      return await _sessionQuerier.ReadAsync(session, cancellationToken);
    }
    catch (Exception)
    {
      _cacheService.Configuration = null;
      throw;
    }
  }

  private static Configuration BuildConfiguration(ConfigurationAggregate configuration, UserAggregate user)
  {
    Actor actor = new(user.FullName ?? user.UniqueName.Value)
    {
      Id = user.Id.AggregateId.ToGuid(),
      Type = ActorType.User,
      EmailAddress = user.Email?.Address,
      PictureUrl = user.Picture?.Value
    };

    return new Configuration(configuration.Secret.Value)
    {
      Version = configuration.Version,
      CreatedBy = actor,
      CreatedOn = configuration.CreatedOn.ToUniversalTime(),
      UpdatedBy = actor,
      UpdatedOn = configuration.UpdatedOn.ToUniversalTime(),
      DefaultLocale = configuration.DefaultLocale?.Code,
      UniqueNameSettings = new Contracts.Settings.UniqueNameSettings(configuration.UniqueNameSettings),
      PasswordSettings = new Contracts.Settings.PasswordSettings(configuration.PasswordSettings),
      RequireUniqueEmail = configuration.RequireUniqueEmail,
      LoggingSettings = new LoggingSettings(configuration.LoggingSettings)
    };
  }
}
