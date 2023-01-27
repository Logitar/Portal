using FluentValidation;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordService : IPasswordService
  {
    private const KeyDerivationPrf Algorithm = KeyDerivationPrf.HMACSHA256;
    private const int IterationCount = 100000;
    private const int SaltLength = 256 / 8;

    private readonly IRepository _repository;

    public PasswordService(IRepository repository)
    {
      _repository = repository;
    }

    public string GenerateAndHash(int length, out byte[] password)
    {
      password = RandomNumberGenerator.GetBytes(length);

      return Hash(Convert.ToBase64String(password));
    }

    public string Hash(string password)
    {
      Pbkdf2 pbkdf2 = new(password, Algorithm, IterationCount, SaltLength);
      return pbkdf2.ToString();
    }

    public bool IsMatch(Session session, byte[] password)
    {
      if (session.KeyHash == null)
      {
        return false;
      }

      Pbkdf2 pbkdf2 = Pbkdf2.Parse(session.KeyHash);
      return pbkdf2.IsMatch(Convert.ToBase64String(password));
    }

    public bool IsMatch(User user, string password)
    {
      if (user.PasswordHash == null)
      {
        return false;
      }

      Pbkdf2 pbkdf2 = Pbkdf2.Parse(user.PasswordHash);
      return pbkdf2.IsMatch(password);
    }

    public void ValidateAndThrow(string password, Configuration configuration) => ValidateAndThrow(password, configuration.PasswordSettings);
    public void ValidateAndThrow(string password, Realm realm) => ValidateAndThrow(password, realm.PasswordSettings);
    private static void ValidateAndThrow(string password, PasswordSettings passwordSettings)
    {
      PasswordValidator validator = new(passwordSettings);
      validator.ValidateAndThrow(password);
    }

    public void ValidateAndThrow(string password)
    {
      throw new NotImplementedException();
    }

    public async Task ValidateAndThrowAsync(string password, string? realmId, CancellationToken cancellationToken)
    {
      if (realmId == null)
      {
        Configuration configuration = await _repository.LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken)
          ?? throw new InvalidOperationException("The configuration could not be found.");

        ValidateAndThrow(password, configuration);
      }
      else
      {
        Realm realm = await _repository.LoadAsync<Realm>(new AggregateId(realmId), cancellationToken)
          ?? throw new InvalidOperationException($"The realm '{realmId}' could not be found.");

        ValidateAndThrow(password, realm);
      }
    }
  }
}
