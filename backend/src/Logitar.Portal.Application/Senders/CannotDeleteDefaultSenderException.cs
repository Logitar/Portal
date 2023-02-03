using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Senders
{
  public class CannotDeleteDefaultSenderException : Exception
  {
    public CannotDeleteDefaultSenderException(Sender sender)
      : base($"The default sender '{sender}' cannot be deleted unless it's alone in its realm.")
    {
      Data["Sender"] = sender.ToString();
    }
  }
}
