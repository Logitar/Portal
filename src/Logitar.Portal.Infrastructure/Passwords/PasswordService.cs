using FluentValidation;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Passwords.Validators;
using Logitar.Portal.Infrastructure.Passwords.Strategies;

namespace Logitar.Portal.Infrastructure.Passwords;

internal class PasswordService : IPasswordService
{
  private readonly Dictionary<string, IPasswordStrategy> _strategies;

  public PasswordService(IEnumerable<IPasswordStrategy> strategies)
  {
    _strategies = strategies.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last());
  }

  public Password Create(IPasswordSettings passwordSettings, string password)
  {
    new PasswordValidator(passwordSettings, "Password").ValidateAndThrow(password);

    return FindStrategy(passwordSettings.Strategy).Create(password);
  }

  public Password Decode(string encoded)
  {
    string key = encoded.Split(Password.Separator).First();

    return FindStrategy(key).Decode(encoded);
  }

  public Password Generate(IPasswordSettings passwordSettings, int length, out byte[] password)
  {
    password = RandomNumberGenerator.GetBytes(length);

    return FindStrategy(passwordSettings.Strategy).Create(Convert.ToBase64String(password));
  }

  private IPasswordStrategy FindStrategy(string key) => _strategies.TryGetValue(key, out IPasswordStrategy? strategy)
    ? strategy : throw new PasswordStrategyNotSupportedException(key);
}
