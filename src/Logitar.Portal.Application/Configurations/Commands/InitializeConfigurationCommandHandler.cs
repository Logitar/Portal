using Logitar.EventSourcing;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand, InitializeConfigurationResult>
{
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordService _passwordService;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public InitializeConfigurationCommandHandler(IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository, IPasswordService passwordService,
    IUserManager userManager, IUserQuerier userQuerier)
  {
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _passwordService = passwordService;
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<InitializeConfigurationResult> Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    if (await _configurationRepository.LoadAsync(cancellationToken) != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    InitializeConfigurationPayload payload = command.Payload;
    Locale locale = payload.Locale.GetLocale(nameof(payload.Locale))
      ?? throw new InvalidLocaleException(payload.Locale, nameof(payload.Locale));
    AggregateId userId = AggregateId.NewId();
    ActorId actorId = new(userId.Value);

    ConfigurationAggregate configuration = new(locale, actorId: actorId);

    UserAggregate user = new(configuration.UniqueNameSettings, payload.User.UniqueName, tenantId: null, actorId, userId);
    user.SetPassword(_passwordService.Create(payload.User.Password));
    user.Email = new EmailAddress(payload.User.EmailAddress);
    user.FirstName = payload.User.FirstName;
    user.LastName = payload.User.LastName;
    user.Locale = locale;

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userManager.SaveAsync(user, cancellationToken);

    return new InitializeConfigurationResult
    {
      Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken),
      User = await _userQuerier.ReadAsync(user, cancellationToken)
    };
  }
}
