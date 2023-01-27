using FluentValidation;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class CustomUserValidator : IUserValidator
  {
    private readonly IRepository _repository;

    public CustomUserValidator(IRepository repository)
    {
      _repository = repository;
    }

    public void ValidateAndThrow(User user, Configuration configuration) => ValidateAndThrow(user, configuration.UsernameSettings);
    public void ValidateAndThrow(User user, Realm realm) => ValidateAndThrow(user, realm.UsernameSettings);
    private static void ValidateAndThrow(User user, UsernameSettings usernameSettings)
    {
      UserValidator validator = new(usernameSettings);
      validator.ValidateAndThrow(user);
    }

    public async Task ValidateAndThrowAsync(User user, CancellationToken cancellationToken)
    {
      if (user.RealmId.HasValue)
      {
        Realm realm = await _repository.LoadAsync<Realm>(user.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm '{user.RealmId}' could not be found.");

        ValidateAndThrow(user, realm);
      }
      else
      {
        Configuration configuration = await _repository.LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken)
          ?? throw new InvalidOperationException("The configuration could not be found.");

        ValidateAndThrow(user, configuration);
      }
    }
  }
}
