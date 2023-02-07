using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Users.Commands
{
  internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public CreateUserCommandHandler(IPasswordService passwordService,
      IRepository repository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserValidator userValidator)
    {
      _passwordService = passwordService;
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
      CreateUserPayload payload = request.Payload;

      Realm? realm = null;
      UsernameSettings? usernameSettings;
      PasswordSettings? passwordSettings;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
        usernameSettings = realm.UsernameSettings;
        passwordSettings = realm.PasswordSettings;
      }
      else
      {
        Configuration configuration = await _repository.LoadConfigurationAsync(cancellationToken)
          ?? throw new InvalidOperationException("The configuration could not be loaded.");
        usernameSettings = configuration.UsernameSettings;
        passwordSettings = configuration.PasswordSettings;
      }

      if (await _repository.LoadUserByUsernameAsync(payload.Username, realm, cancellationToken) != null)
      {
        throw new UsernameAlreadyUsedException(payload.Username, nameof(payload.Username));
      }
      else if (realm?.RequireUniqueEmail == true
        && payload.Email != null
        && (await _repository.LoadUsersByEmailAsync(payload.Email, realm, cancellationToken)).Any())
      {
        throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
      }

      string? passwordHash = null;
      if (payload.Password != null)
      {
        _passwordService.ValidateAndThrow(payload.Password, passwordSettings);
        passwordHash = _passwordService.Hash(payload.Password);
      }

      CultureInfo? locale = payload.Locale?.GetCultureInfo();
      User user = new(_userContext.ActorId, payload.Username, realm, passwordHash,
        payload.Email, payload.IsEmailConfirmed, payload.PhoneNumber, payload.IsPhoneNumberConfirmed,
        payload.FirstName, payload.MiddleName, payload.LastName,
        locale, payload.Picture);
      _userValidator.ValidateAndThrow(user, usernameSettings);

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
