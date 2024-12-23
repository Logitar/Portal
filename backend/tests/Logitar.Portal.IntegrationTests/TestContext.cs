﻿using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal;

internal record TestContext
{
  public RealmModel? Realm { get; set; }
  public ApiKeyModel? ApiKey { get; set; }
  public UserModel? User { get; set; }
  public SessionModel? Session { get; set; }
}
