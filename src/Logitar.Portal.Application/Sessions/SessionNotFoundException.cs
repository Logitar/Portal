using Logitar.EventSourcing;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  public SessionNotFoundException(AggregateId id) : base($"The session 'Id={id}' could not be found.")
  {
    Id = id.Value;
  }

  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
}
