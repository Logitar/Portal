namespace Logitar.Portal.Infrastructure2.Handlers.Configurations
{
  internal static class ConfigurationKeys
  {
    public static string DefaultLocale { get; } = nameof(DefaultLocale);
    public static string JwtSecret { get; } = nameof(JwtSecret);

    public static string UsernameSettings_AllowedCharacters { get; } = nameof(UsernameSettings_AllowedCharacters).Replace('_', '.');

    public static string PasswordSettings_RequiredLength { get; } = nameof(PasswordSettings_RequiredLength).Replace('_', '.');
    public static string PasswordSettings_RequiredUniqueChars { get; } = nameof(PasswordSettings_RequiredUniqueChars).Replace('_', '.');
    public static string PasswordSettings_RequireNonAlphanumeric { get; } = nameof(PasswordSettings_RequireNonAlphanumeric).Replace('_', '.');
    public static string PasswordSettings_RequireLowercase { get; } = nameof(PasswordSettings_RequireLowercase).Replace('_', '.');
    public static string PasswordSettings_RequireUppercase { get; } = nameof(PasswordSettings_RequireUppercase).Replace('_', '.');
    public static string PasswordSettings_RequireDigit { get; } = nameof(PasswordSettings_RequireDigit).Replace('_', '.');
  }
}
