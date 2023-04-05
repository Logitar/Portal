using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class Password
  {
    private const char Delimiter = ':';

    private readonly KeyDerivationPrf _algorithm;
    private readonly int _iterations;
    private readonly byte[] _salt;
    private readonly byte[] _hash;

    public Password(string password, byte[] salt, int iterations, KeyDerivationPrf algorithm, int? hashLength = null)
    {
      ArgumentNullException.ThrowIfNull(password);

      _salt = salt ?? throw new ArgumentNullException(nameof(salt));
      _iterations = iterations;
      _algorithm = algorithm;

      _hash = Hash(password, hashLength ?? salt.Length);
    }
    private Password(KeyDerivationPrf algorith, int iterations, byte[] salt, byte[] hash)
    {
      _algorithm = algorith;
      _iterations = iterations;
      _salt = salt ?? throw new ArgumentNullException(nameof(salt));
      _hash = hash ?? throw new ArgumentNullException(nameof(hash));
    }

    public static Password Parse(string s)
    {
      ArgumentNullException.ThrowIfNull(s);

      string[] parts = s.Split(Delimiter);
      if (parts.Length != 4)
      {
        throw new ArgumentException($"The value '{s}' is not a valid password.", nameof(s));
      }

      return new Password(
        Enum.Parse<KeyDerivationPrf>(parts[0]),
        int.Parse(parts[1]),
        Convert.FromBase64String(parts[2]),
        Convert.FromBase64String(parts[3])
      );
    }
    public static bool TryParse(string s, out Password? password)
    {
      ArgumentNullException.ThrowIfNull(s);

      try
      {
        password = Parse(s);
        return true;
      }
      catch (Exception)
      {
        password = null;
        return false;
      }
    }

    public bool IsMatch(string password) => _hash.SequenceEqual(Hash(password, _hash.Length));

    private byte[] Hash(string password, int length) => KeyDerivation.Pbkdf2(password, _salt, KeyDerivationPrf.HMACSHA256, _iterations, length);

    public override bool Equals(object? obj) => obj is Password password
      && password._algorithm == _algorithm
      && password._iterations == _iterations
      && password._salt.SequenceEqual(_salt)
      && password._hash.SequenceEqual(_hash);
    public override int GetHashCode() => HashCode.Combine(_algorithm, _iterations, _salt, _hash);
    public override string ToString() => string.Join(Delimiter, _algorithm, _iterations, Convert.ToBase64String(_salt), Convert.ToBase64String(_hash));
  }
}
