using Logitar.Identity.Domain.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys;

internal record XApiKey
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "PT";
  private const char Separator = '.';

  public ApiKeyId Id { get; }
  public string Secret { get; }

  public XApiKey(ApiKeyId id, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentException($"The secret must contain exactly {SecretLength} bytes.", nameof(secret));
    }

    Id = id;
    Secret = secret;
  }

  public static XApiKey Decode(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid X-API-Key.", nameof(value));
    }

    ApiKeyId id = new(values[1]);
    string secret = values[2].FromUriSafeBase64();
    return new XApiKey(id, secret);
  }
  public static string Encode(ApiKeyId id, string secret) => new XApiKey(id, secret).Encode();

  public string Encode() => string.Join(Separator, Prefix, Id.Value, Secret.ToUriSafeBase64());
}
