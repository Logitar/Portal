﻿namespace Logitar.Portal.Contracts.Users;

public record CreateUserPayload
{
  public string? Id { get; set; }

  public string UniqueName { get; set; }
  public string? Password { get; set; }
  public bool IsDisabled { get; set; }

  public CreateUserPayload() : this(string.Empty)
  {
  }

  public CreateUserPayload(string uniqueName, string? password = null)
  {
    UniqueName = uniqueName;
    Password = password;
  }
}
