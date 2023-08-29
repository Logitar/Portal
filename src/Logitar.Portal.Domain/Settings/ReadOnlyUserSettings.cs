using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyUserSettings : IUserSettings
{
  public ReadOnlyUserSettings(bool requireUniqueEmail = false, bool requireConfirmedAccount = false,
    IUniqueNameSettings? uniqueNameSettings = null, IPasswordSettings? passwordSettings = null)
  {
    RequireUniqueEmail = requireUniqueEmail;
    RequireConfirmedAccount = requireConfirmedAccount;

    UniqueNameSettings = uniqueNameSettings ?? new ReadOnlyUniqueNameSettings();
    PasswordSettings = passwordSettings ?? new ReadOnlyPasswordSettings();
  }

  public bool RequireUniqueEmail { get; }
  public bool RequireConfirmedAccount { get; }

  public IUniqueNameSettings UniqueNameSettings { get; }
  public IPasswordSettings PasswordSettings { get; }
}
