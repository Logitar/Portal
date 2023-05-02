using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.Client;

internal class ConfigurationService : HttpService, IConfigurationService
{
  public ConfigurationService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public Task<Configuration?> GetAsync(CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }

  public async Task<Configuration> InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken)
    => await PostAsync<Configuration>("configurations", input, cancellationToken);

  public Task<Configuration> UpdateAsync(UpdateConfigurationInput input, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }
}
