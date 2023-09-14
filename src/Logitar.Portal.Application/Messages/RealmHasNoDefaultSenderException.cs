using FluentValidation.Results;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Messages;

public class RealmHasNoDefaultSenderException : Exception
{
  private const string ErrorMessage = "The specified realm has no default sender.";

  public RealmHasNoDefaultSenderException(RealmAggregate? realm, string propertyName)
    : base(BuildMessage(realm, propertyName))
  {
    Realm = realm?.ToString();
    PropertyName = propertyName;
  }

  public string? Realm
  {
    get => (string?)Data[nameof(Realm)];
    private set => Data[nameof(Realm)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, attemptedValue: null)
  {
    ErrorCode = "RealmHasNoDefaultSender"
  };

  private static string BuildMessage(RealmAggregate? realm, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Realm: ").AppendLine(realm?.ToString() ?? "<null>");
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
