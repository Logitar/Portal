using FluentValidation;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Sessions
{
  internal class SignInService : ISignInService
  {
    private const int SessionKeyLength = 32;

    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IValidator<Session> _sessionValidator;
    private readonly IUserValidator _userValidator;

    public SignInService(IPasswordService passwordService,
      IRepository repository,
      ISessionQuerier sessionQuerier,
      IValidator<Session> sessionValidator,
      IUserValidator userValidator)
    {
      _passwordService = passwordService;
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionValidator = sessionValidator;
      _userValidator = userValidator;
    }

    public async Task<SessionModel> SignInAsync(User user, Realm? realm, bool remember, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      UsernameSettings usernameSettings = realm?.UsernameSettings
        ?? (await _repository.LoadConfigurationAsync(cancellationToken))?.UsernameSettings
        ?? throw new InvalidOperationException("The username settings could not be resolved.");

      byte[]? keyBytes = null;
      string? keyHash = null;
      if (remember)
      {
        keyHash = _passwordService.GenerateAndHash(SessionKeyLength, out keyBytes);
      }

      Session session = new(user, keyHash, ipAddress, additionalInformation);
      _sessionValidator.ValidateAndThrow(session);

      user.SignIn();
      _userValidator.ValidateAndThrow(user, usernameSettings);

      await _repository.SaveAsync(new AggregateRoot[] { session, user }, cancellationToken);

      SessionModel model = await _sessionQuerier.GetAsync(session.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The session '{session.Id}' could not be found.");

      model.RenewToken = keyBytes == null ? null : new SecureToken(model.Id, keyBytes).ToString();

      return model;
    }
  }
}
