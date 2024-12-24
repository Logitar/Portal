using Logitar.Portal.Application.Logging;
using Logitar.Portal.MongoDB.Entities;
using MongoDB.Driver;

namespace Logitar.Portal.MongoDB.Repositories;

internal class LogRepository : ILogRepository
{
  private readonly IMongoCollection<LogEntity> _logs;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public LogRepository(IMongoDatabase database)
  {
    _logs = database.GetCollection<LogEntity>("logs");
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public async Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    LogEntity entity = new(log, _serializerOptions);

    await _logs.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
  }
}
