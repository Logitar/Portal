namespace Logitar.Portal.Contracts.Configurations;

public interface IConfigurationClient
{
  Task<ConfigurationModel> ReadAsync(IRequestContext? context = null);
  Task<ConfigurationModel> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, IRequestContext? context = null);
  Task<ConfigurationModel> UpdateAsync(UpdateConfigurationPayload payload, IRequestContext? context = null);
}
