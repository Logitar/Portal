﻿using Logitar.Portal.Contracts.Users;
using System.Globalization;

namespace Logitar.Portal.Contracts.Realms
{
  public record CreateRealmPayload
  {
    public string Alias { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public CultureInfo? DefaultLocale { get; set; }
    public string? Url { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public bool RequireUniqueEmail { get; set; }

    public UsernameSettingsPayload UsernameSettings { get; set; } = new();
    public PasswordSettingsPayload PasswordSettings { get; set; } = new();

    public string JwtSecret { get; set; } = string.Empty;

    public string? GoogleClientId { get; set; }
  }
}
