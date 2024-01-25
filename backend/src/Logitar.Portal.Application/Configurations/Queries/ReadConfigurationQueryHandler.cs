﻿using Logitar.Portal.Application.Caching;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal class ReadConfigurationQueryHandler : IRequestHandler<ReadConfigurationQuery, Configuration?>
{
  private readonly ICacheService _cacheService;

  public ReadConfigurationQueryHandler(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task<Configuration?> Handle(ReadConfigurationQuery _, CancellationToken cancellationToken)
  {
    Configuration? configuration = _cacheService.GetConfiguration();
    return Task.FromResult(configuration);
  }
}
