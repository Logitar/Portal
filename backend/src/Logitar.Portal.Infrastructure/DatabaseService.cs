using Logitar.Portal.Core.Actors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Infrastructure
{
  internal class DatabaseService : IDatabaseService
  {
    private readonly IActorService _actorService;
    private readonly IConfiguration _configuration;
    private readonly PortalDbContext _dbContext;

    public DatabaseService(
      IActorService actorService,
      IConfiguration configuration,
      PortalDbContext dbContext
    )
    {
      _actorService = actorService;
      _configuration = configuration;
      _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
      if (_configuration.GetValue<bool>("MigrateDatabase"))
      {
        await _dbContext.Database.MigrateAsync(cancellationToken);
      }

      if (_configuration.GetValue<bool>("SynchronizeActors"))
      {
        await _actorService.SynchronizeAsync(cancellationToken);
      }
    }
  }
}
