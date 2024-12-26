using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  public const string ErrorMessage = "The default sender cannot be deleted, unless its the only sender in its Realm.";

  public SenderId SenderId
  {
    get => new((string)Data[nameof(SenderId)]!);
    private set => Data[nameof(SenderId)] = value.Value;
  }

  public CannotDeleteDefaultSenderException(Sender sender) : base(BuildMessage(sender))
  {
    SenderId = sender.Id;
  }

  private static string BuildMessage(Sender sender) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id.Value)
    .Build();
}
