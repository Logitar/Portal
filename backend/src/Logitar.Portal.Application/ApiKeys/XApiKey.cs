using Logitar.Portal.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.Application.ApiKeys
{
  public readonly struct XApiKey
  {
    private const string AllowedIdCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
    private const string Prefix = "PT";
    private const char Separator = '.';

    public XApiKey(AggregateId id, byte[] secret)
    {
      if (id.Value.Any(c => !AllowedIdCharacters.Contains(c)))
      {
        throw new ArgumentException($"The identifier can only contain the following characters: {AllowedIdCharacters}", nameof(id));
      }
      if (!secret.Any())
      {
        throw new ArgumentException("The secret is required.", nameof(secret));
      }

      Id = id;
      Secret = secret;
    }

    public AggregateId Id { get; }
    public byte[] Secret { get; }

    public static XApiKey Parse(string s)
    {
      string[] parts = s.Split(Separator);
      if (parts.Length != 3 || parts[0] != Prefix)
      {
        throw new ArgumentException($"The value '{s}' is not a valid X-API-Key.", nameof(s));
      }

      return new XApiKey(new AggregateId(parts[1]), Convert.FromBase64String(parts[2].FromUriSafeHash()));
    }
    public static bool TryParse(string s, out XApiKey xApiKey)
    {
      try
      {
        xApiKey = Parse(s);
        return true;
      }
      catch (Exception)
      {
        xApiKey = default;
        return false;
      }
    }

    public static bool operator ==(XApiKey x, XApiKey y) => x.Equals(y);
    public static bool operator !=(XApiKey x, XApiKey y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is XApiKey key
      && key.Id == Id
      && key.Secret.SequenceEqual(Secret);
    public override int GetHashCode() => HashCode.Combine(Id, Secret);
    public override string ToString() => string.Join(Separator,
      Prefix,
      Id.Value,
      Convert.ToBase64String(Secret).ToUriSafeHash());
  }
}
