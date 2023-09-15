namespace Logitar.Portal.Domain.Messages;

public class SendMessageResult
{
  private readonly Dictionary<string, string> _data = new();

  public SendMessageResult()
  {
  }

  public SendMessageResult(int capacity) : this()
  {
    _data = new(capacity);
  }

  public SendMessageResult(IEnumerable<KeyValuePair<string, string>> data) : this(data.Count())
  {
    foreach (KeyValuePair<string, string> pair in data)
    {
      _data[pair.Key] = pair.Value;
    }
  }

  public Dictionary<string, string> AsDictionary() => new(_data);
}
