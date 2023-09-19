namespace Logitar.Portal.Contracts.Settings;

public interface IPasswordSettings
{
  int RequiredLength { get; }
  int RequiredUniqueChars { get; }
  bool RequireNonAlphanumeric { get; }
  bool RequireLowercase { get; }
  bool RequireUppercase { get; }
  bool RequireDigit { get; }

  string Strategy { get; }
}
