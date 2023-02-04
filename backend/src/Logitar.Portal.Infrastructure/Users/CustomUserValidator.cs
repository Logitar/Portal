using FluentValidation;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class CustomUserValidator : IUserValidator
  {
    private readonly IValidator<ExternalProvider> _externalProviderValidator;
    private readonly IRepository _repository;

    public CustomUserValidator(IValidator<ExternalProvider> externalProviderValidator, IRepository repository)
    {
      _externalProviderValidator = externalProviderValidator;
      _repository = repository;
    }

    public void ValidateAndThrow(User user, UsernameSettings usernameSettings)
    {
      UserValidator validator = new(_externalProviderValidator, usernameSettings);
      validator.ValidateAndThrow(user);
    }

    public async Task ValidateAndThrowAsync(User user, CancellationToken cancellationToken)
    {
      UsernameSettings usernameSettings = (user.RealmId.HasValue
        ? (await _repository.LoadAsync<Realm>(user.RealmId.Value, cancellationToken))?.UsernameSettings
        : (await _repository.LoadConfigurationAsync(cancellationToken))?.UsernameSettings)
        ?? throw new InvalidOperationException("The username settings could not be resolved.");

      ValidateAndThrow(user, usernameSettings);
    }
  }
}
