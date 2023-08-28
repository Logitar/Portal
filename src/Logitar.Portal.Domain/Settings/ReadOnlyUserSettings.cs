using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyUserSettings : IUserSettings
{
  public ReadOnlyUserSettings(bool requireUniqueEmail = false, bool requireConfirmedAccout = false,
    IUniqueNameSettings? uniqueNameSettings = null, IPasswordSettings? passwordSettings = null)
  {
    RequireUniqueEmail = requireUniqueEmail;
    RequireConfirmedAccount = requireConfirmedAccout;

    UniqueNameSettings = uniqueNameSettings ?? new ReadOnlyUniqueNameSettings();
    PasswordSettings = passwordSettings ?? new ReadOnlyPasswordSettings();
  }

  public bool RequireUniqueEmail { get; }
  public bool RequireConfirmedAccount { get; }

  public IUniqueNameSettings UniqueNameSettings { get; }
  public IPasswordSettings PasswordSettings { get; }
}
