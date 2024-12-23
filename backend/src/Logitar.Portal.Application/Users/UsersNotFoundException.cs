﻿namespace Logitar.Portal.Application.Users;

public class UsersNotFoundException : Exception
{
  public const string ErrorMessage = "The specified users could not be found.";

  public IReadOnlyCollection<Guid> Ids
  {
    get => (IReadOnlyCollection<Guid>)Data[nameof(Ids)]!;
    private set => Data[nameof(Ids)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UsersNotFoundException(IEnumerable<Guid> ids, string? propertyName = null) : base(BuildMessage(ids, propertyName))
  {
    Ids = ids.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(IEnumerable<Guid> ids, string? propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(Ids)).AppendLine(":");
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }
    return message.ToString();
  }
}
