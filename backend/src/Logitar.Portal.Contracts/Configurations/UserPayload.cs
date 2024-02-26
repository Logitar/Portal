﻿namespace Logitar.Portal.Contracts.Configurations;

public record UserPayload
{
  public string UniqueName { get; set; }
  public string Password { get; set; }

  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }

  public UserPayload() : this(string.Empty, string.Empty)
  {
  }

  public UserPayload(string uniqueName, string password)
  {
    UniqueName = uniqueName;
    Password = password;
  }
}
