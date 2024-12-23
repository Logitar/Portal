using Logitar.Identity.Core.Sessions;

namespace Logitar.Portal.Application.Sessions;

internal record RefreshToken
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "RT";
  private const char Separator = '.';

  public SessionId Id { get; }
  public string Secret { get; }

  public RefreshToken(SessionId id, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentException($"The secret must contain exactly {SecretLength} bytes.", nameof(secret));
    }

    Id = id;
    Secret = secret;
  }

  public static RefreshToken Decode(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    SessionId id = new(values[1]);
    string secret = values[2].FromUriSafeBase64();
    return new RefreshToken(id, secret);
  }
  public static string Encode(SessionId id, string secret) => new RefreshToken(id, secret).Encode();

  public string Encode() => string.Join(Separator, Prefix, Id.Value, Secret.ToUriSafeBase64());
}
