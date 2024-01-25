using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations.Validators;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand, Session>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationCommandHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository, IPasswordManager passwordManager, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _passwordManager = passwordManager;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<Session> Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    InitializeConfigurationPayload payload = command.Payload;
    new InitializeConfigurationValidator().ValidateAndThrow(payload);

    if (await _configurationRepository.LoadAsync(cancellationToken) != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    UserId userId = UserId.NewId();
    ActorId actorId = new(userId.Value);

    LocaleUnit locale = new(payload.Locale);
    ConfigurationAggregate configuration = ConfigurationAggregate.Initialize(locale, actorId);
    Configuration model = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    _cacheService.SetConfiguration(model);

    UserPayload userPayload = payload.User;
    UniqueNameUnit uniqueName = new(configuration.UniqueNameSettings, userPayload.UniqueName);
    UserAggregate user = new(uniqueName, tenantId: null, actorId, userId)
    {
      FirstName = PersonNameUnit.TryCreate(userPayload.FirstName),
      LastName = PersonNameUnit.TryCreate(userPayload.LastName),
      Locale = locale
    };
    user.Update(actorId);
    if (userPayload.Email != null)
    {
      user.SetEmail(new EmailUnit(userPayload.Email.Address), actorId);
    }
    Password password = _passwordManager.ValidateAndCreate(userPayload.Password);
    user.SetPassword(password, actorId);

    SessionPayload sessionPayload = payload.Session;
    SessionAggregate session = user.SignIn();
    foreach (CustomAttribute customAttribute in sessionPayload.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(actorId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    return await _sessionQuerier.ReadAsync(session, cancellationToken);
  }
}
