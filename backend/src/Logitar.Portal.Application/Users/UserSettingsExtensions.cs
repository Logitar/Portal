using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal static class UserSettingsExtensions
  {
    public static UsernameSettings GetUsernameSettings(this UsernameSettingsPayload payload) => new()
    {
      AllowedCharacters = payload.AllowedCharacters
    };

    public static PasswordSettings GetPasswordSettings(this PasswordSettingsPayload payload) => new()
    {
      RequiredLength = payload.RequiredLength,
      RequiredUniqueChars = payload.RequiredUniqueChars,
      RequireNonAlphanumeric = payload.RequireNonAlphanumeric,
      RequireLowercase = payload.RequireLowercase,
      RequireUppercase = payload.RequireUppercase,
      RequireDigit = payload.RequireDigit
    };
  }
}
