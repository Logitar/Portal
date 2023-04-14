using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal class IsConfigurationInitializedHandler : IRequestHandler<IsConfigurationInitialized, bool>
{
  private readonly IConfigurationRepository _configurationRepository;

  public IsConfigurationInitializedHandler(IConfigurationRepository configurationRepository)
  {
    _configurationRepository = configurationRepository;
  }

  public async Task<bool> Handle(IsConfigurationInitialized request, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);

    return configuration != null;
  }
}
