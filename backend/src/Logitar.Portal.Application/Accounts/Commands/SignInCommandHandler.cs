using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class SignInCommandHandler : IRequestHandler<SignInCommand, SessionModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly ISignInService _signInService;

    public SignInCommandHandler(IPasswordService passwordService,
      IRepository repository,
      ISignInService signInService)
    {
      _passwordService = passwordService;
      _repository = repository;
      _signInService = signInService;
    }

    public async Task<SessionModel> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (request.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(request.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(request.Realm);
      }

      SignInPayload payload = request.Payload;

      User? user = await _repository.LoadUserByUsernameAsync(payload.Username, realm, cancellationToken);
      if (user == null || !_passwordService.IsMatch(user, payload.Password))
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.SignInAsync(user, realm, payload.Remember, payload.IpAddress, payload.AdditionalInformation, cancellationToken);
    }
  }
}
