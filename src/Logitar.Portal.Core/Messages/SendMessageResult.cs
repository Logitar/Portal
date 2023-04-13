namespace Logitar.Portal.Core.Messages;

public class SendMessageResult : Dictionary<string, string>
{
  public SendMessageResult() : base()
  {
  }
  public SendMessageResult(int capacity) : base(capacity)
  {
  }
}
