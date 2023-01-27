using FluentValidation;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Sessions
{
  internal class SignInService : ISignInService
  {
    private const int SessionKeyLength = 32;

    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IValidator<Session> _sessionValidator;

    public SignInService(IPasswordService passwordService,
      IRepository repository,
      ISessionQuerier sessionQuerier,
      IValidator<Session> sessionValidator)
    {
      _passwordService = passwordService;
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionValidator = sessionValidator;
    }

    public async Task<SessionModel> RenewAsync(Session session, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      string keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out byte[] keyBytes);
      session.Renew(keyHash, ipAddress, additionalInformation);
      _sessionValidator.ValidateAndThrow(session);

      await _repository.SaveAsync(session, cancellationToken);

      SessionModel model = await _sessionQuerier.GetAsync(session.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={session.Id}' could not be found.");

      model.RenewToken = new SecureToken(model.Id.FromHash(), keyBytes).ToString();

      return model;
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
      _sessionValidator.ValidateAndThrow(session);

      user.SignIn();

      await _repository.SaveAsync(new AggregateRoot[] { session, user }, cancellationToken);

      SessionModel model = await _sessionQuerier.GetAsync(session.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={session.Id}' could not be found.");

      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id.FromHash(), keyBytes).ToString();

      return model;
    }
  }
}
