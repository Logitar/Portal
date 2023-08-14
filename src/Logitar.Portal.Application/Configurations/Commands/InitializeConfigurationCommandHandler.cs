using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand, Unit>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordService _passwordService;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository, IPasswordService passwordService,
    IUserManager userManager)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _passwordService = passwordService;
    _userManager = userManager;
  }

  public async Task<Unit> Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    InitializeConfigurationPayload payload = command.Payload;
    CultureInfo locale = payload.Locale.GetCultureInfo(nameof(payload.Locale)) ?? CultureInfo.InvariantCulture;
    AggregateId userId = AggregateId.NewId();
    ActorId actorId = new(userId.Value);

    configuration = new(locale, actorId);
    _applicationContext.Configuration = configuration;

    UserAggregate user = new(configuration.UniqueNameSettings, payload.User.UniqueName, tenantId: null, actorId, userId);
    user.SetPassword(_passwordService.Create(payload.User.Password));
    user.Email = new EmailAddress(payload.User.EmailAddress);
    user.FirstName = payload.User.FirstName;
    user.LastName = payload.User.LastName;
    user.Locale = locale;
    user.Update(actorId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userManager.SaveAsync(user, cancellationToken);

    return Unit.Value;
  }
}
