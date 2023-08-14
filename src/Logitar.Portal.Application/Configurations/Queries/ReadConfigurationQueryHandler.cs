using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, Configuration>
{
  private readonly IApplicationContext _applicationContext;

  public ReadConfigurationQueryHandler(IApplicationContext applicationContext)
  {
    _applicationContext = applicationContext;
  }

  public async Task<Configuration> Handle(ReadConfigurationQuery query, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = _applicationContext.Configuration;

    throw new NotImplementedException(); // TODO(fpion): map aggregate to output model
  }
}
