using Logitar.Portal.Core2.Sessions.Models;
using Logitar.Portal.Core2.Tokens;
using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Core2.Sessions
{
  internal class SignInService : ISignInService
  {
    private const int SessionKeyLength = 32;

    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;

    public SignInService(IPasswordService passwordService,
      IRepository repository,
      ISessionQuerier sessionQuerier)
    {
      _passwordService = passwordService;
      _repository = repository;
      _sessionQuerier = sessionQuerier;
    }

    public async Task<SessionModel> SignInAsync(User user, bool remember, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      byte[]? keyBytes = null;
      string? keyHash = null;
      if (remember)
      {
        keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out keyBytes);
      }

      Session session = new(user, keyHash, ipAddress, additionalInformation);
      user.SignIn();
      await _repository.SaveAsync(new AggregateRoot[] { session, user }, cancellationToken);

      SessionModel model = await _sessionQuerier.GetAsync(session.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={session.Id}' could not be found.");

      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id.FromHash(), keyBytes).ToString();

      return model;
    }
  }
}
