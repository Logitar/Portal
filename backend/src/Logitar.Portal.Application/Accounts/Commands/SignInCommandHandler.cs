using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class SignInCommandHandler : IRequestHandler<SignInCommand, SessionModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRealmRepository _realmRepository;
    private readonly ISignInService _signInService;
    private readonly IUserRepository _userRepository;

    public SignInCommandHandler(IPasswordService passwordService,
      IRealmRepository realmRepository,
      ISignInService signInService,
      IUserRepository userRepository)
    {
      _passwordService = passwordService;
      _realmRepository = realmRepository;
      _signInService = signInService;
      _userRepository = userRepository;
    }

    public async Task<SessionModel> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (request.Realm != null)
      {
        realm = await _realmRepository.LoadByAliasOrIdAsync(request.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(request.Realm);
      }

      SignInPayload payload = request.Payload;

      User? user = await _userRepository.LoadByUsernameAsync(payload.Username, realm?.Id, cancellationToken);
      if (user == null || !_passwordService.IsMatch(user, payload.Password))
      {
        throw new InvalidCredentialsException();
      }
      user.EnsureIsTrusted(realm);

      return await _signInService.SignInAsync(user, realm, payload.Remember, payload.IpAddress, payload.AdditionalInformation, cancellationToken);
    }
  }
}
