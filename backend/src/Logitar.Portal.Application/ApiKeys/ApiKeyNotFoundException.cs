using Logitar.Identity.Domain.ApiKeys;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.ApiKeys;

public class ApiKeyNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified API key could not be found.";

  public ApiKeyId Id
  {
    get => new((string)Data[nameof(Id)]!);
    private set => Data[nameof(Id)] = value.Value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public ApiKeyNotFoundException(ApiKeyId id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(ApiKeyId id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
