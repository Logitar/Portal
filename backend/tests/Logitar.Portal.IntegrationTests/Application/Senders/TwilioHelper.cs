namespace Logitar.Portal.Application.Senders;
internal static class TwilioHelper
{
  public static string GenerateAccountSid() => string.Concat("AC", Guid.NewGuid().ToString("n"));
  public static string GenerateAuthenticationToken() => Guid.NewGuid().ToString("n");
}
