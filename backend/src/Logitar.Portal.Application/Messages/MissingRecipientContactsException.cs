namespace Logitar.Portal.Application.Messages;

public class MissingRecipientContactsException : Exception
{
  private const string ErrorMessage = "The specified recipients are missing an email address.";

  public IReadOnlyCollection<Guid> UserIds
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(UserIds)]!;
    private set => Data[nameof(UserIds)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public MissingRecipientContactsException(IEnumerable<Guid> userIds, string propertyName) : base(BuildMessage(userIds, propertyName))
  {
    UserIds = userIds.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> userIds, string propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(UserIds)).AppendLine(":");
    foreach (Guid userId in userIds)
    {
      message.Append(" - ").Append(userId).AppendLine();
    }
    return message.ToString();
  }
}
