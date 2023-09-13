using FluentValidation.Results;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Senders;

public class SenderNotInRealmException : Exception
{
  private const string ErrorMessage = "The specified sender must be in the specified realm.";

  public SenderNotInRealmException(SenderAggregate sender, RealmAggregate? realm, string propertyName)
    : base(BuildMessage(sender, realm, propertyName))
  {
    Sender = sender.ToString();
    Realm = realm?.ToString();
    PropertyName = propertyName;
  }

  public string Sender
  {
    get => (string)Data[nameof(Sender)]!;
    private set => Data[nameof(Sender)] = value;
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

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Sender)
  {
    ErrorCode = "SenderNotInRealm",
    CustomState = new { Realm }
  };

  private static string BuildMessage(SenderAggregate sender, RealmAggregate? realm, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Sender: ").Append(sender).AppendLine();
    message.Append("Realm: ").AppendLine(realm?.ToString() ?? "<null>");
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
