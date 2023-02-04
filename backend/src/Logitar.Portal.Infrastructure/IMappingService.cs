namespace Logitar.Portal.Infrastructure
{
  internal interface IMappingService
  {
    Task<T?> MapAsync<T>(object? source, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> MapAsync<T>(IEnumerable<object?> sources, CancellationToken cancellationToken = default);
  }
}
