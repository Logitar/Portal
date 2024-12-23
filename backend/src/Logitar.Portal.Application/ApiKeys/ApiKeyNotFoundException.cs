using Logitar.Identity.Core;
using Logitar.Identity.Core.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

public class ApiKeyNotFoundException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified API key could not be found.";

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

  public ApiKeyNotFoundException(ApiKeyId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    Id = id.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ApiKeyId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
