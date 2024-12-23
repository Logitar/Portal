using Logitar.Identity.Core;
using Logitar.Identity.Core.Sessions;

namespace Logitar.Portal.Application.Sessions;

public class SessionNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified session could not be found.";

  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public SessionNotFoundException(SessionId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    Id = id.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(SessionId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
