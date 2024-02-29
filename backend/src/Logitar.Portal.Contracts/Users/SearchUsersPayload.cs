﻿using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Users;

public record SearchUsersPayload : SearchPayload
{
  public bool? HasAuthenticated { get; set; }
  public bool? HasPassword { get; set; }
  public bool? IsDisabled { get; set; }
  public bool? IsConfirmed { get; set; }

  public new List<UserSortOption> Sort { get; set; } = [];
}
