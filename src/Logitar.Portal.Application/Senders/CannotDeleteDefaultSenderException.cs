using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  public CannotDeleteDefaultSenderException(SenderAggregate sender)
    : base($"The default sender '{sender}' cannot be deleted, unless it is alone in its realm.")
  {
    Sender = sender.ToString();
  }

  public string Sender
  {
    get => (string)Data[nameof(Sender)]!;
    private set => Data[nameof(Sender)] = value;
  }
}
