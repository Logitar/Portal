using Logitar.Portal.Domain.Passwords;

namespace Logitar.Portal.Infrastructure.Passwords.Strategies;

internal class Pbkdf2Strategy : IPasswordStrategy
{
  private readonly IPbkdf2Settings _settings;

  public Pbkdf2Strategy(IPbkdf2Settings settings)
  {
    _settings = settings;
  }

  public string Key => Pbkdf2.Prefix;

  public Password Create(byte[] password) => Create(Convert.ToBase64String(password));
  public Password Create(string password)
  {
    byte[] salt = RandomNumberGenerator.GetBytes(_settings.SaltLength);

    return new Pbkdf2(password, _settings.Algorithm, _settings.Iterations, salt, _settings.HashLength);
  }

  public Password Decode(string encoded) => Pbkdf2.Decode(encoded);
}
