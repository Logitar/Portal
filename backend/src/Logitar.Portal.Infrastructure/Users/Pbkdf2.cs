using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class Pbkdf2
  {
    private const char Separator = ':';

    private readonly KeyDerivationPrf _algorith;
    private readonly int _iterations;
    private readonly byte[] _salt;
    private readonly byte[] _hash;

    public Pbkdf2(string password)
    {
      _algorith = KeyDerivationPrf.HMACSHA256;
      _iterations = 100000;
      _salt = RandomNumberGenerator.GetBytes(32);
      _hash = ComputeHash(password);
    }
    private Pbkdf2(KeyDerivationPrf algorith, int iterations, byte[] salt, byte[] hash)
    {
      _algorith = algorith;
      _iterations = iterations;
      _salt = salt;
      _hash = hash;
    }

    public static Pbkdf2 Parse(string s)
    {
      string[] values = s.Split(Separator);
      if (values.Length != 4)
      {
        throw new ArgumentException($"The value '{s}' is not a valid Pbkdf2 representation.", nameof(s));
      }

      return new Pbkdf2(Enum.Parse<KeyDerivationPrf>(values[0]),
        int.Parse(values[1]),
        Convert.FromBase64String(values[2]),
        Convert.FromBase64String(values[3]));
    }
    public static bool TryParse(string s, out Pbkdf2? pbkdf2)
    {
      try
      {
        pbkdf2 = Parse(s);
        return true;
      }
      catch (Exception)
      {
        pbkdf2 = null;
        return false;
      }
    }

    public bool IsMatch(string password)
      => _hash.SequenceEqual(ComputeHash(password, _hash.Length));

    private byte[] ComputeHash(string password, int? length = null)
      => KeyDerivation.Pbkdf2(password, _salt, _algorith, _iterations, length ?? _salt.Length);

    public override bool Equals(object? obj) => obj is Pbkdf2 pbkdf2
      && pbkdf2._algorith == _algorith
      && pbkdf2._iterations == _iterations
      && pbkdf2._salt.SequenceEqual(_salt)
      && pbkdf2._hash.SequenceEqual(_hash);
    public override int GetHashCode() => HashCode.Combine(_algorith, _iterations, _salt, _hash);
    public override string ToString() => string.Join(Separator,
      _algorith,
      _iterations,
      Convert.ToBase64String(_salt),
      Convert.ToBase64String(_hash));
  }
}
