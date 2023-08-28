using Logitar.EventSourcing;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand, InitializeConfigurationResult>
{
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordService _passwordService;
  private readonly ISessionQuerier _sessionQuerier;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationCommandHandler(IConfigurationQuerier configurationQuerier, IConfigurationRepository configurationRepository,
    IPasswordService passwordService, ISessionQuerier sessionQuerier, ISessionRepository sessionRepository, IUserRepository userRepository)
  {
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _passwordService = passwordService;
    _sessionQuerier = sessionQuerier;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
  }

  public async Task<InitializeConfigurationResult> Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    InitializeConfigurationPayload payload = command.Payload;
    Locale locale = payload.Locale.GetRequiredLocale(nameof(payload.Locale));

    AggregateId userId = AggregateId.NewId();
    ActorId actorId = new(userId.ToGuid());

    configuration = new(locale, actorId);

    UserAggregate user = new(configuration.UniqueNameSettings, payload.User.UniqueName, tenantId: null, actorId, userId)
    {
      Email = new EmailAddress(payload.User.EmailAddress),
      FirstName = payload.User.FirstName,
      LastName = payload.User.LastName,
      Locale = locale
    };
    user.SetPassword(_passwordService.Create(configuration.PasswordSettings, payload.User.Password));
    user.Update(actorId);

    byte[]? secretBytes = null;
    Password? secret = payload.Session.IsPersistent
      ? _passwordService.Generate(configuration.PasswordSettings, SessionAggregate.SecretLength, out secretBytes)
      : null;
    SessionAggregate session = user.SignIn(configuration.UserSettings, secret);
    foreach (CustomAttribute customAttribute in payload.Session.CustomAttributes)
    {
      session.SetCustomAttribute(customAttribute.Key, customAttribute.Value);
    }
    session.Update(actorId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userRepository.SaveAsync(user, cancellationToken);
    await _sessionRepository.SaveAsync(session, cancellationToken);

    Session sessionResult = await _sessionQuerier.ReadAsync(session, cancellationToken);
    if (secretBytes != null)
    {
      sessionResult.RefreshToken = new RefreshToken(session, secretBytes).Encode();
    }

    return new InitializeConfigurationResult
    {
      Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken),
      Session = sessionResult
    };
  }
}
