using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public ChangePasswordCommandHandler(IPasswordService passwordService,
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

    public async Task<UserModel> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(_userContext.UserId, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.UserId}' could not be found.");

      Realm? realm = user.RealmId.HasValue
        ? (await _repository.LoadAsync<Realm>(user.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={user.RealmId}' could not be found."))
        : null;
      Configuration? configuration = realm == null
        ? await _repository.LoadConfigurationAsync(cancellationToken)
        : null;

      _passwordService.ValidateAndThrow(request.Payload.Password, realm?.PasswordSettings ?? configuration?.PasswordSettings ?? new());

      if (!_passwordService.IsMatch(user, request.Payload.Current))
      {
        throw new InvalidCredentialsException();
      }

      string passwordHash = _passwordService.Hash(request.Payload.Password);
      user.ChangePassword(passwordHash);
      _userValidator.ValidateAndThrow(user, realm?.UsernameSettings ?? configuration?.UsernameSettings ?? new());

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
