namespace Portal.Core
{
  public interface IUserContext
  {
    Guid ActorId { get; }
    Guid Id { get; }
    Guid SessionId { get; }

    string BaseUrl { get; }
  }
}
