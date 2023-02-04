using Logitar.Portal.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.Application.Sessions
{
  internal readonly struct RenewToken
  {
    private const char Separator = '.';

    public RenewToken(string id, byte[] key)
    {
      if (string.IsNullOrWhiteSpace(id))
      {
        throw new ArgumentException("The ID is required.", nameof(id));
      }
      if (!key.Any())
      {
        throw new ArgumentException("The key is required.", nameof(key));
      }

      Id = id;
      Key = key;
    }

    public string Id { get; }
    public byte[] Key { get; }

    public static RenewToken Parse(string s)
    {
      string[] values = s.Split(Separator);
      if (values.Length != 2)
      {
        throw new ArgumentException($"The value '{s}' is not a valid renew token.", nameof(s));
      }

      return new RenewToken(values[0], Convert.FromBase64String(values[1].FromUriSafeHash()));
    }
    public static bool TryParse(string s, out RenewToken token)
    {
      try
      {
        token = Parse(s);
        return true;
      }
      catch (Exception)
      {
        token = default;
        return false;
      }
    }

    public static bool operator ==(RenewToken x, RenewToken y) => x.Equals(y);
    public static bool operator !=(RenewToken x, RenewToken y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is RenewToken token
      && token.Id == Id
      && token.Key.SequenceEqual(Key);
    public override int GetHashCode() => HashCode.Combine(Id, Key);
    public override string ToString() => string.Join(Separator, Id, Convert.ToBase64String(Key).ToUriSafeHash());
  }
}
