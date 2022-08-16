namespace Logitar.Portal.Application.Accounts
{
  public class SecureToken
  {
    private const int GuidByteCount = 16;

    public SecureToken(Guid id, byte[] key)
    {
      Id = id;
      Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public Guid Id { get; }
    public byte[] Key { get; }

    public static SecureToken Parse(string s)
    {
      ArgumentNullException.ThrowIfNull(s);

      byte[] bytes = Convert.FromBase64String(s);

      return new SecureToken(
        id: new Guid(bytes.Take(GuidByteCount).ToArray()),
        key: bytes.Skip(GuidByteCount).ToArray()
      );
    }
    public static bool TryParse(string s, out SecureToken? secureToken)
    {
      ArgumentNullException.ThrowIfNull(s);

      try
      {
        secureToken = Parse(s);
        return true;
      }
      catch (Exception)
      {
        secureToken = null;
        return false;
      }
    }

    public override bool Equals(object? obj) => obj is SecureToken token && token.Id == Id && token.Key == Key;
    public override int GetHashCode() => HashCode.Combine(GetType(), Id, Key);
    public override string ToString() => Convert.ToBase64String(Id.ToByteArray().Concat(Key).ToArray());
  }
}
