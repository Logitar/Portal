using AutoMapper;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMapper _mapper;

  public ReadConfigurationQueryHandler(IApplicationContext applicationContext, IMapper mapper)
  {
    _applicationContext = applicationContext;
    _mapper = mapper;
  }

  public Task<Configuration> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    ConfigurationAggregate aggregate = _applicationContext.Configuration;

    Configuration configuration = _mapper.Map<Configuration>(aggregate);

    return Task.FromResult(configuration);
  }
}
