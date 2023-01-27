using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.Core2.Tokens
{
  internal readonly struct SecureToken
  {
    public SecureToken(Guid id, byte[] key)
    {
      Id = id;
      Key = key;
    }

    public Guid Id { get; }
    public byte[] Key { get; }

    public static bool operator ==(SecureToken x, SecureToken y) => x.Equals(y);
    public static bool operator !=(SecureToken x, SecureToken y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is SecureToken token
      && token.Id == Id
      && token.Key.SequenceEqual(Key);
    public override int GetHashCode() => HashCode.Combine(Id, Key);
    public override string ToString() => Convert.ToBase64String(Id.ToByteArray().Concat(Key).ToArray());
  }
}
