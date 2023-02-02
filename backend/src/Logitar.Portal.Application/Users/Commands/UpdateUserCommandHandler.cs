using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Users.Commands
{
  internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;

    public UpdateUserCommandHandler(IPasswordService passwordService,
      IRepository repository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserRepository userRepository,
      IUserValidator userValidator)
    {
      _passwordService = passwordService;
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userRepository = userRepository;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<User>(request.Id);

      Realm? realm = user.RealmId.HasValue
        ? (await _repository.LoadAsync<Realm>(user.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={user.RealmId}' could not be found."))
        : null;
      Configuration? configuration = realm == null
        ? await _repository.LoadConfigurationAsync(cancellationToken)
        : null;

      UpdateUserPayload payload = request.Payload;

      if (realm?.RequireUniqueEmail == true && payload.Email != null)
      {
        IEnumerable<User> users = await _userRepository.LoadByEmailAsync(payload.Email, realm, cancellationToken);
        if (users.Any(x => !x.Equals(user)))
        {
          throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
        }
      }

      string? passwordHash = null;
      if (payload.Password != null)
      {
        _passwordService.ValidateAndThrow(payload.Password, realm?.PasswordSettings ?? configuration?.PasswordSettings ?? new());
        passwordHash = _passwordService.Hash(payload.Password);
      }

      CultureInfo? locale = payload.Locale == null ? null : CultureInfo.GetCultureInfo(payload.Locale);
      user.Update(_userContext.ActorId, passwordHash,
        payload.Email, payload.PhoneNumber,
        payload.FirstName, payload.MiddleName, payload.LastName,
        locale, payload.Picture);
      _userValidator.ValidateAndThrow(user, realm?.UsernameSettings ?? configuration?.UsernameSettings ?? new());

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
