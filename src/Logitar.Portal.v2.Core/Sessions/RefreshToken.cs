using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Portal.v2.Core.Sessions;

internal readonly struct RefreshToken
{
  private const int GuidByteCount = 16;

  public RefreshToken(Guid id, byte[] key)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("The identifier is required.", nameof(id));
    }
    if (key.Length == 0)
    {
      throw new ArgumentException("The key is required.", nameof(key));
    }

    Id = id;
    Key = key;
  }

  public Guid Id { get; }
  public byte[] Key { get; }

  public static RefreshToken Parse(string s)
  {
    byte[] bytes = Convert.FromBase64String(s.FromUriSafeBase64());

    return new RefreshToken(new Guid(bytes.Take(GuidByteCount).ToArray()), bytes.Skip(GuidByteCount).ToArray());
  }
  public static bool TryParse(string s, out RefreshToken refreshToken)
  {
    try
    {
      refreshToken = Parse(s);
      return true;
    }
    catch (Exception)
    {
      refreshToken = default;
      return false;
    }
  }

  public static bool operator ==(RefreshToken left, RefreshToken right) => left.Equals(right);
  public static bool operator !=(RefreshToken left, RefreshToken right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RefreshToken refreshToken
    && refreshToken.Id == Id
    && refreshToken.Key.SequenceEqual(Key);
  public override int GetHashCode() => HashCode.Combine(Id, Key);
  public override string ToString() => Convert.ToBase64String(Id.ToByteArray().Concat(Key).ToArray())
    .ToUriSafeBase64();
}
