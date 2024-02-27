using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Contracts.Configurations;

public interface IConfigurationClient
{
  Task<Session> InitializeAsync(InitializeConfigurationPayload payload, IRequestContext? context = null);
  Task<Configuration?> ReadAsync(IRequestContext? context = null);
  Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, IRequestContext? context = null);
  Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, IRequestContext? context = null);
}
