using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.Core.Tokens
{
  internal readonly struct SecureToken
  {
    private const int GuidByteCount = 16;

    public SecureToken(Guid id, byte[] key)
    {
      Id = id;
      Key = key;
    }

    public Guid Id { get; }
    public byte[] Key { get; }

    public static bool TryParse(string s, out SecureToken secureToken)
    {
      try
      {
        secureToken = Parse(s);
        return true;
      }
      catch (Exception)
      {
        secureToken = default;
        return false;
      }
    }
    public static SecureToken Parse(string s)
    {
      byte[] bytes = Convert.FromBase64String(s);

      return new SecureToken(new Guid(bytes.Take(GuidByteCount).ToArray()),
        bytes.Skip(GuidByteCount).ToArray());
    }

    public static bool operator ==(SecureToken x, SecureToken y) => x.Equals(y);
    public static bool operator !=(SecureToken x, SecureToken y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is SecureToken token
      && token.Id == Id
      && token.Key.SequenceEqual(Key);
    public override int GetHashCode() => HashCode.Combine(Id, Key);
    public override string ToString() => Convert.ToBase64String(Id.ToByteArray().Concat(Key).ToArray());
  }
}
