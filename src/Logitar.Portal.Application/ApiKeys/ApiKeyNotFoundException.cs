using Logitar.EventSourcing;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.ApiKeys;

public class ApiKeyNotFoundException : InvalidCredentialsException
{
  public ApiKeyNotFoundException(AggregateId id) : base($"The API key 'Id={id}' could not be found.")
  {
    Id = id.Value;
  }

  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
}
