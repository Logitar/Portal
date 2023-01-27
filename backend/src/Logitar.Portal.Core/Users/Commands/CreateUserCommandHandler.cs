using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserModel>
  {
    private readonly IPasswordService _passwordService;
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public CreateUserCommandHandler(IPasswordService passwordService,
      IRealmQuerier realmQuerier,
      IRepository repository,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserValidator userValidator)
    {
      _passwordService = passwordService;
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
      CreateUserPayload payload = request.Payload;

      RealmModel? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(payload.Realm, cancellationToken)
          ?? throw EntityNotFoundException.Typed<Realm>(payload.Realm, nameof(payload.Realm));
      }

      if (await _userQuerier.GetAsync(payload.Username, realm?.Id, cancellationToken) != null)
      {
        throw new UsernameAlreadyUsedException(payload.Username, nameof(payload.Username));
      }
      else if (payload.Email != null
        && realm?.RequireUniqueEmail == true
        && (await _userQuerier.GetByEmailAsync(payload.Email, realm.Id, cancellationToken)).Any())
      {
        throw new EmailAlreadyUsedException(payload.Email, nameof(payload.Email));
      }

      string? passwordHash = null;
      if (payload.Password != null)
      {
        await _passwordService.ValidateAndThrowAsync(payload.Password, realm?.Id, cancellationToken);
        passwordHash = _passwordService.Hash(payload.Password);
      }

      CultureInfo? locale = payload.Locale == null ? null : CultureInfo.GetCultureInfo(payload.Locale);
      User user = new(realm?.Id, payload.Username, passwordHash, payload.Email, payload.PhoneNumber,
        payload.FirstName, payload.MiddleName, payload.LastName, locale, payload.Picture, _userContext.ActorId);
      if (payload.ConfirmEmail)
      {
        user.ConfirmEmail(_userContext.ActorId);
      }
      if (payload.ConfirmPhoneNumber)
      {
        user.ConfirmPhoneNumber(_userContext.ActorId);
      }
      await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

      await _repository.SaveAsync(user, cancellationToken);

      return await _userQuerier.GetAsync(user.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
