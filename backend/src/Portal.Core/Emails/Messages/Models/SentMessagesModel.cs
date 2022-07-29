namespace Portal.Core.Emails.Messages.Models
{
  public class SentMessagesModel
  {
    public SentMessagesModel(IEnumerable<Message> messages)
    {
      ArgumentNullException.ThrowIfNull(messages);

      var success = new List<Guid>(capacity: messages.Count());
      var error = new List<Guid>(success.Capacity);
      var unsent = new List<Guid>(success.Capacity);

      foreach (Message message in messages)
      {
        if (message.Succeeded)
        {
          success.Add(message.Id);
        }
        else if (message.HasErrors)
        {
          error.Add(message.Id);
        }
        else
        {
          unsent.Add(message.Id);
        }
      }

      Success = success;
      Error = error;
      Unsent = unsent;
    }

    public IEnumerable<Guid> Success { get; }
    public IEnumerable<Guid> Error { get; }
    public IEnumerable<Guid> Unsent { get; }
  }
}
