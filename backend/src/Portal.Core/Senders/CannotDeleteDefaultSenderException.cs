using System.Net;
using System.Text;

namespace Portal.Core.Senders
{
  internal class CannotDeleteDefaultSenderException : ApiException
  {
    public CannotDeleteDefaultSenderException(Guid id, Guid actorId)
      : base(HttpStatusCode.BadRequest, GetMessage(id, actorId))
    {
      ActorId = actorId;
      Id = id;

      Value = new { code = "CannotDeleteDefaultSender" };
    }

    public Guid ActorId { get; }
    public Guid Id { get; }

    private static string GetMessage(Guid id, Guid actorId)
    {
      var message = new StringBuilder();

      message.AppendLine("The default sender cannot be deleted unless it's alone in its realm.");
      message.AppendLine($"Sender ID: {id}");
      message.AppendLine($"Actor ID: {actorId}");

      return message.ToString();
    }
  }
}
