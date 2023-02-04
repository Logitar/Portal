using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
  {
    private readonly IInternalTokenService _internalTokenService;
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserValidator _userValidator;

    public ResetPasswordCommandHandler(IInternalTokenService internalTokenService,
      IPasswordService passwordService,
      IRepository repository,
      IUserValidator userValidator)
    {
      _internalTokenService = internalTokenService;
      _passwordService = passwordService;
      _repository = repository;
      _userValidator = userValidator;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadRealmByAliasOrIdAsync(request.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(request.Realm);

      ResetPasswordPayload payload = request.Payload;

      _passwordService.ValidateAndThrow(payload.Password, realm.PasswordSettings);

      ValidateTokenPayload validateTokenPayload = new()
      {
        Token = payload.Token,
        Purpose = ResetPassword.Purpose,
        Realm = realm.Id.Value
      };
      ValidatedTokenModel token = await _internalTokenService.ValidateAsync(validateTokenPayload, consume: true, cancellationToken);
      if (!token.Succeeded)
      {
        Exception? innerException = null;
        if (token.Errors.Count() == 1)
        {
          ErrorModel model = token.Errors.Single();
          Error error = new(model.Code, model.Description, model.Data?.ToDictionary(x => x.Key, x => x.Value));
          innerException = new ErrorException(error);
        }

        throw new InvalidCredentialsException(innerException);
      }
      else if (token.Subject == null)
      {
        throw new InvalidOperationException($"The {nameof(token.Subject)} is required.");
      }

      User user = await _repository.LoadAsync<User>(token.Subject, cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={token.Subject}' could not be found.");
      user.EnsureIsTrusted(realm);

      string passwordHash = _passwordService.Hash(payload.Password);
      user.ChangePassword(passwordHash);
      _userValidator.ValidateAndThrow(user, realm.UsernameSettings);

      await _repository.SaveAsync(user, cancellationToken);

      return Unit.Value;
    }
  }
}
