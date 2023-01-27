namespace Logitar.Portal.Infrastructure2
{
  internal interface IMappingService
  {
    Task<T?> MapAsync<T>(object? source, CancellationToken cancellationToken = default);
  }
}
