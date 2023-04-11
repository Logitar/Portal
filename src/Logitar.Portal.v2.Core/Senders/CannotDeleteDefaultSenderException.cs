using Logitar.EventSourcing;
using System.Text;

namespace Logitar.Portal.v2.Core.Senders;

public class CannotDeleteDefaultSenderException : Exception
{
  public CannotDeleteDefaultSenderException(SenderAggregate sender, AggregateId actorId)
    : base(GetMessage(sender, actorId))
  {
    Data["Sender"] = sender.ToString();
    Data["ActorId"] = actorId.Value;
  }

  private static string GetMessage(SenderAggregate sender, AggregateId actorId)
  {
    StringBuilder message = new();

    message.AppendLine("The default sender cannot be deleted unless it's alone in its realm.");
    message.Append("Sender: ").Append(sender).AppendLine();
    message.Append("ActorId: ").Append(actorId).AppendLine();

    return message.ToString();
  }
}
