﻿namespace Logitar.Portal.Application.Messages;

[Obsolete($"Do not use this class. It will be removed in the next major release. It has been replaced by the {nameof(MissingRecipientContactsException)} class.")]
public class MissingRecipientAddressesException : Exception
{
  public const string ErrorMessage = "The specified recipients are missing an email address.";

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

  public MissingRecipientAddressesException(IEnumerable<Guid> userIds, string propertyName) : base(BuildMessage(userIds, propertyName))
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
