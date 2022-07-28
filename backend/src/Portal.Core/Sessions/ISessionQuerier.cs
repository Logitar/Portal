namespace Portal.Core.Sessions
{
  public interface ISessionQuerier
  {
    Task<Session?> GetAsync(Guid id, bool readOnly = false, CancellationToken cancellationToken = default);
  }
}
