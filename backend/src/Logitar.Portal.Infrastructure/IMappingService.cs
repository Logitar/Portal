namespace Logitar.Portal.Infrastructure
{
  internal interface IMappingService
  {
    Task<T?> MapAsync<T>(object? source, CancellationToken cancellationToken = default);
  }
}
