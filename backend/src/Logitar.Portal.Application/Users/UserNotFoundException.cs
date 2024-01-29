﻿using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.Users;

public class UserNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified user could not be found.";

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string User
  {
    get => (string)Data[nameof(User)]!;
    private set => Data[nameof(User)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UserNotFoundException(string? tenantId, string user, string? propertyName = null) : base(BuildMessage(tenantId, user, propertyName))
  {
    TenantId = tenantId;
    User = user;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string? tenantId, string user, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId, "<null>")
    .AddData(nameof(User), user)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
