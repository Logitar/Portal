using FluentValidation.Results;

namespace Logitar.Portal.Domain.ApiKeys;

public class CannotPostponeExpirationException : Exception, IFailureException
{
  private const string ErrorMessage = "The API key expiration cannot be postponed.";

  public CannotPostponeExpirationException(ApiKeyAggregate apiKey, DateTime? expiresOn, string propertyName)
    : base(BuildMessage(apiKey, expiresOn, propertyName))
  {
    ApiKey = apiKey.ToString();
    ExpiresOn = expiresOn;
    PropertyName = propertyName;
  }

  public string ApiKey
  {
    get => (string)Data[nameof(ApiKey)]!;
    private set => Data[nameof(ApiKey)] = value;
  }
  public DateTime? ExpiresOn
  {
    get => (DateTime?)Data[nameof(ExpiresOn)];
    private set => Data[nameof(ExpiresOn)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, ExpiresOn)
  {
    ErrorCode = "CannotPostponeExpiration"
  };

  private static string BuildMessage(ApiKeyAggregate apiKey, DateTime? expiresOn, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("ApiKey: ").AppendLine(apiKey.ToString());
    message.Append("ExpiresOn: ").Append(expiresOn?.ToString() ?? "<null>").AppendLine();
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
