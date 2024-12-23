using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  public const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender in its Realm.";

  public string SenderId
  {
    get => (string)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
  }

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    SenderId = sender.Id.Value;
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id)
    .Build();
}
