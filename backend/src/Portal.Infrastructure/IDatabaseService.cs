namespace Portal.Infrastructure
{
  public interface IDatabaseService
  {
    Task InitializeAsync(CancellationToken cancellationToken = default);
  }
}
