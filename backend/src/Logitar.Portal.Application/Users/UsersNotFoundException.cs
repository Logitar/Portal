﻿using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Users;

public class UsersNotFoundException : Exception
{
  private const string ErrorMessage = "The specified users could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
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

  public UsersNotFoundException(TenantId? tenantId, IEnumerable<Guid> ids, string? propertyName = null) : base(BuildMessage(tenantId, ids, propertyName))
  {
    TenantId = tenantId?.ToGuid();
    Ids = ids.ToArray();
    PropertyName = propertyName;
  }

  private static string BuildMessage(TenantId? tenantId, IEnumerable<Guid> ids, string? propertyName)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(TenantId)).Append(": ").AppendLine(tenantId?.ToGuid().ToString() ?? "<null>");
    message.Append(nameof(PropertyName)).Append(": ").AppendLine(propertyName ?? "<null>");
    message.Append(nameof(Ids)).AppendLine(":");
    foreach (Guid id in ids)
    {
      message.Append(" - ").Append(id).AppendLine();
    }
    return message.ToString();
  }
}
