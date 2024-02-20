using System.Security.Cryptography;

namespace Logitar.Portal.Application.Senders;

internal static class SendGridHelper
{
  public static string GenerateApiKey() => string.Join('.', "SG",
    Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToUriSafeBase64(),
    Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).ToUriSafeBase64());
}
