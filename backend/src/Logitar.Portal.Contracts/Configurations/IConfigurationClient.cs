namespace Logitar.Portal.Contracts.Configurations;

public interface IConfigurationClient
{
  Task<Configuration> ReadAsync(IRequestContext? context = null);
  Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, IRequestContext? context = null);
  Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, IRequestContext? context = null);
}
