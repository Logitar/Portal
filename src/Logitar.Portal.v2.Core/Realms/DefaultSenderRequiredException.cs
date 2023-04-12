﻿namespace Logitar.Portal.v2.Core.Realms;

public class DefaultSenderRequiredException : Exception
{
  public DefaultSenderRequiredException(RealmAggregate realm)
    : base($"The default sender is required for the realm '{realm}'.")
  {
    Data["Realm"] = realm.ToString();
  }
}
