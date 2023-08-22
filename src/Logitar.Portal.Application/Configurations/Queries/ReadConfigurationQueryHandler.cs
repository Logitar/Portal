using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, Configuration?>
{
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;

  public ReadConfigurationQueryHandler(IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository)
  {
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
  }

  public async Task<Configuration?> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);
    if (configuration == null)
    {
      return null;
    }

    return await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
