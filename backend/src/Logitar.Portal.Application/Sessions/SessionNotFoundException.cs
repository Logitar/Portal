using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified session could not be found.";

  public SessionId Id
  {
    get => new((string)Data[nameof(Id)]!);
    private set => Data[nameof(Id)] = value.Value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public SessionNotFoundException(SessionId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(SessionId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
