using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Infrastructure
{
  internal class DatabaseService : IDatabaseService
  {
    private readonly IConfiguration _configuration;
    private readonly PortalDbContext _dbContext;

    public DatabaseService(IConfiguration configuration, PortalDbContext dbContext)
    {
      _configuration = configuration;
      _dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
      if (_configuration.GetValue<bool>("MigrateDatabase"))
      {
        await _dbContext.Database.MigrateAsync(cancellationToken);
      }
    }
  }
}
