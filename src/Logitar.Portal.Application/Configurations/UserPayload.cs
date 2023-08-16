﻿namespace Logitar.Portal.Application.Configurations;

public record UserPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public string EmailAddress { get; set; } = string.Empty;

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
}
