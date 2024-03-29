﻿using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Activities;

public record ContextParameters : IContextParameters
{
  public Realm? Realm { get; set; }
  public ApiKey? ApiKey { get; set; }
  public User? User { get; set; }
  public Session? Session { get; set; }

  public string? RealmOverride { get; set; }
  public string? ImpersonifiedUser { get; set; }
}
