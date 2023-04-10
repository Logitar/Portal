﻿namespace Logitar.Portal.v2.Core.Users;

public class AccountIsNotConfirmedException : Exception
{
  public AccountIsNotConfirmedException(UserAggregate user)
    : base($"The user account '{user}' is not confirmed.")
  {
    Data["User"] = user.ToString();
  }
}