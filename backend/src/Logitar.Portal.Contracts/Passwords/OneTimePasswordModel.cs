﻿using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Passwords;

public class OneTimePasswordModel : AggregateModel
{
  public string? Password { get; set; }

  public DateTime? ExpiresOn { get; set; }
  public int? MaximumAttempts { get; set; }

  public int AttemptCount { get; set; }
  public bool HasValidationSucceeded { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public RealmModel? Realm { get; set; }
}
