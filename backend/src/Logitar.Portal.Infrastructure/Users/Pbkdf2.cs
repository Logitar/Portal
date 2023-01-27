using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure.Users
{
  internal readonly struct Pbkdf2
  {
    private const char Separator = ';';

    private readonly KeyDerivationPrf _algorithm;
    private readonly int _iterations;
    private readonly byte[] _salt;
    private readonly byte[] _hash;

    public Pbkdf2(string password, KeyDerivationPrf algorithm, int iterations, int saltLength)
    {
      _algorithm = algorithm;
      _iterations = iterations;
      _salt = RandomNumberGenerator.GetBytes(saltLength);
      _hash = Hash(password);
    }
    private Pbkdf2(KeyDerivationPrf algorithm, int iterations, byte[] salt, byte[] hash)
    {
      _algorithm = algorithm;
      _iterations = iterations;
      _salt = salt;
      _hash = hash;
    }

    public static bool TryParse(string s, out Pbkdf2 pbkdf2)
    {
      try
      {
        pbkdf2 = Parse(s);
        return true;
      }
      catch (Exception)
      {
        pbkdf2 = default;
        return false;
      }
    }
    public static Pbkdf2 Parse(string s)
    {
      string[] values = s.Split(Separator);
      if (s.Length != 4)
      {
        throw new ArgumentException($"The value '{s}' is not a valid Pbkdf2 string.");
      }

      return new Pbkdf2(Enum.Parse<KeyDerivationPrf>(values[0]),
        int.Parse(values[1]),
        Convert.FromBase64String(values[2]),
        Convert.FromBase64String(values[3]));
    }

    public bool IsMatch(string password) => _hash.SequenceEqual(Hash(password, _hash.Length));

    private byte[] Hash(string password, int? length = null)
      => KeyDerivation.Pbkdf2(password, _salt, _algorithm, _iterations, length ?? _salt.Length);

    public static bool operator ==(Pbkdf2 x, Pbkdf2 y) => x.Equals(y);
    public static bool operator !=(Pbkdf2 x, Pbkdf2 y) => !x.Equals(y);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Pbkdf2 pbkdf2
      && pbkdf2._algorithm == _algorithm
      && pbkdf2._iterations == _iterations
      && pbkdf2._salt.SequenceEqual(_salt)
      && pbkdf2._hash.SequenceEqual(_hash);
    public override int GetHashCode() => HashCode.Combine(_algorithm, _iterations, _salt, _hash);
    public override string ToString() => string.Join(Separator,
      _algorithm,
      _iterations,
      Convert.ToBase64String(_salt),
      Convert.ToBase64String(_hash));
  }
}
