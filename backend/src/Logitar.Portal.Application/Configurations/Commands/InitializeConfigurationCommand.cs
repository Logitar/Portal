using Logitar.EventSourcing;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

public record InitializeConfigurationCommand(string UniqueName, string Password) : INotification;

internal class InitializeConfigurationCommandHandler : INotificationHandler<InitializeConfigurationCommand>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;

  public InitializeConfigurationCommandHandler(ICacheService cacheService, IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository, IPasswordManager passwordManager, IUserManager userManager)
  {
    _cacheService = cacheService;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
    _passwordManager = passwordManager;
    _userManager = userManager;
  }

  public async Task Handle(InitializeConfigurationCommand command, CancellationToken cancellationToken)
  {
    Configuration? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration == null)
    {
      UserId userId = UserId.NewId();
      ActorId actorId = new(userId.Value);

      configuration = Configuration.Initialize(actorId);
      UserSettings userSettings = new()
      {
        UniqueName = configuration.UniqueNameSettings,
        Password = configuration.PasswordSettings,
        RequireUniqueEmail = configuration.RequireUniqueEmail
      };

      UniqueNameUnit uniqueName = new(userSettings.UniqueName, command.UniqueName);
      UserAggregate user = new(uniqueName, tenantId: null, actorId, userId);

      Password password = _passwordManager.ValidateAndCreate(command.Password, userSettings.Password);
      user.SetPassword(password, actorId);

      await _userManager.SaveAsync(user, userSettings, actorId, cancellationToken);
      await _configurationRepository.SaveAsync(configuration, cancellationToken); // NOTE(fpion): this should cache the configuration.
    }
    else
    {
      _cacheService.Configuration = await _configurationQuerier.ReadAsync(configuration, cancellationToken);
    }
  }
}
