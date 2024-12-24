using Logitar.Portal.Application.Logging;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Repositories;

internal class LogRepository : ILogRepository
{
  private readonly PortalContext _context;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public LogRepository(PortalContext context)
  {
    _context = context;
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public async Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    LogEntity entity = new(log, _serializerOptions);

    _context.Logs.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
