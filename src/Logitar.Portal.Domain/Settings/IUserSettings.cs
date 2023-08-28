using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings;

public interface IUserSettings
{
  bool RequireUniqueEmail { get; }
  bool RequireConfirmedAccount { get; }

  IUniqueNameSettings UniqueNameSettings { get; }
  IPasswordSettings PasswordSettings { get; }
}
