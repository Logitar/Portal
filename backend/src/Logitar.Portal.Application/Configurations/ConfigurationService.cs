using Logitar.Portal.Application.Configurations.Payloads;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Application.Configurations
{
  internal class ConfigurationService : IConfigurationService
  {
    private readonly IUserService _userService;

    public ConfigurationService(IUserService userService)
    {
      _userService = userService;
    }

    public async Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(payload);

      if (await IsInitializedAsync(cancellationToken))
      {
        throw new ConfigurationAlreadyInitializedException();
      }

      await _userService.CreateAsync(payload.User, cancellationToken);
    }

    public async Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default)
    {
      ListModel<UserModel> users = await _userService.GetAsync(realm: null, count: 1, cancellationToken: cancellationToken);

      return users.Items.Any();
    }
  }
}
