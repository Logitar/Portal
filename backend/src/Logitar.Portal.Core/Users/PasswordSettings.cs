﻿namespace Logitar.Portal.Core.Users
{
  public class PasswordSettings
  {
    public int RequiredLength { get; init; }
    public int RequiredUniqueChars { get; init; }
    public bool RequireNonAlphanumeric { get; init; }
    public bool RequireLowercase { get; init; }
    public bool RequireUppercase { get; init; }
    public bool RequireDigit { get; init; }
  }
}
