using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
  {
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException(); // TODO(fpion): implement

      //RealmModel realm = await _realmQuerier.GetAsync(request.Realm, cancellationToken)
      //  ?? throw EntityNotFoundException.Typed<Realm>(request.Realm, nameof(request.Realm));

      //_passwordService.ValidateAndThrow(payload.Password, realm);

      //ValidateTokenPayload validateTokenPayload = new()
      //{
      //  Token = payload.Token,
      //  Purpose = PasswordResetPurpose,
      //  Realm = realm.Id.ToString()
      //};
      //ValidatedTokenModel token = await _tokenService.ValidateAsync(validateTokenPayload, consume: true, cancellationToken);
      //if (!token.Succeeded)
      //{
      //  Exception? innerException = null;
      //  if (token.Errors.Count() == 1)
      //  {
      //    ErrorModel model = token.Errors.Single();
      //    var error = new Error(model.Code, model.Description, model.Data?.ToDictionary(x => x.Key, x => x.Value));
      //    innerException = new ErrorException(error);
      //  }

      //  throw new InvalidCredentialsException(innerException);
      //}
      //Guid userId = Guid.Parse(token.Subject!);

      //User user = await _userQuerier.GetAsync(userId, cancellationToken)
      //  ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
      //user.EnsureIsTrusted(realm);

      //string passwordHash = _passwordService.Hash(payload.Password);
      //user.ChangePassword(passwordHash);
      //await _repository.SaveAsync(user, cancellationToken);

      //return Unit.Value;
    }
  }
}
