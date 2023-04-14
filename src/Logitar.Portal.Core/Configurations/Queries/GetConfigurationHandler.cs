using AutoMapper;
using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal class GetConfigurationHandler : IRequestHandler<GetConfiguration, Configuration?>
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMapper _mapper;

  public GetConfigurationHandler(IConfigurationRepository configurationRepository, IMapper mapper)
  {
    _configurationRepository = configurationRepository;
    _mapper = mapper;
  }

  public async Task<Configuration?> Handle(GetConfiguration request, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);

    return _mapper.Map<Configuration>(configuration);
  }
}
