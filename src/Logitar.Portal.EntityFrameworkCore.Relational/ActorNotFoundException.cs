using Logitar.EventSourcing;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public class ActorNotFoundException : Exception
{
  public ActorNotFoundException(ActorId id) : base($"The actor 'Id={id}' could not be found.")
  {
    Id = id.Value;
  }

  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
}
