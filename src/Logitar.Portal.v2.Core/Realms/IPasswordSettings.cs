namespace Logitar.Portal.v2.Core.Realms;

internal interface IPasswordSettings
{
  int RequiredLength { get; }
  int RequiredUniqueChars { get; }
  bool RequireNonAlphanumeric { get; }
  bool RequireLowercase { get; }
  bool RequireUppercase { get; }
  bool RequireDigit { get; }
}
