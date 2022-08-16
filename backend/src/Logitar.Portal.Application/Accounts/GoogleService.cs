using FluentValidation;
using Google.Apis.Auth;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Core.Accounts;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Payloads;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Accounts
{
  internal class GoogleService : IGoogleService
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly ISignInService _signInService;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IRepository<User> _userRepository;
    private readonly IValidator<User> _userValidator;

    public GoogleService(
      IRealmQuerier realmQuerier,
      ISignInService signInService,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IRepository<User> userRepository,
      IValidator<User> userValidator
    )
    {
      _realmQuerier = realmQuerier;
      _signInService = signInService;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userRepository = userRepository;
      _userValidator = userValidator;
    }

    public async Task<SessionModel> AuthenticateAsync(string realmId, AuthenticateWithGooglePayload payload, string? ipAddress, string? additionalInformation, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(realmId);
      ArgumentNullException.ThrowIfNull(payload);

      Realm realm = await _realmQuerier.GetAsync(realmId, readOnly: false, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(realmId);

      if (realm.GoogleClientId == null)
      {
        throw new GoogleClientIdRequiredException(realm);
      }

      var validationSettings = new GoogleJsonWebSignature.ValidationSettings
      {
        Audience = new string[] { realm.GoogleClientId }
      };
      GoogleJsonWebSignature.Payload googlePayload = await GoogleJsonWebSignature.ValidateAsync(payload.Credential, validationSettings);

      User? user = await _userQuerier.GetByExternalProviderAsync(realm, ExternalProviders.Google, googlePayload.Subject, readOnly: false, cancellationToken);
      if (user == null)
      {
        user = await _userQuerier.GetAsync(googlePayload.Email, realm, readOnly: false, cancellationToken);
        if (user == null)
        {
          if (realm.RequireUniqueEmail)
          {
            user = (await _userQuerier.GetByEmailAsync(googlePayload.Email, realm, readOnly: false, cancellationToken))
              .SingleOrDefault();
          }

          if (user == null)
          {
            var userPayload = new CreateUserSecurePayload
            {
              ConfirmEmail = googlePayload.EmailVerified,
              Email = googlePayload.Email,
              FirstName = googlePayload.GivenName,
              LastName = googlePayload.FamilyName,
              Locale = payload.IgnoreProviderLocale ? payload.Locale : (googlePayload.Locale ?? payload.Locale),
              Picture = googlePayload.Picture,
              Realm = realmId,
              Username = googlePayload.Email
            };

            user = new User(userPayload, _userContext.Actor.Id, realm);
            if (userPayload.ConfirmEmail)
            {
              user.ConfirmEmail(_userContext.Actor.Id);
            }

            var context = ValidationContext<User>.CreateWithOptions(user, options => options.ThrowOnFailures());
            context.SetAllowedUsernameCharacters(realm.AllowedUsernameCharacters);
            _userValidator.Validate(context);

            await _userRepository.SaveAsync(user, cancellationToken);
          }
        }

        user.AddExternalProvider(ExternalProviders.Google, googlePayload.Subject, _userContext.Actor.Id, ExternalProviders.Google);

        await _userRepository.SaveAsync(user, cancellationToken);
      }

      return await _signInService.SignInAsync(user, remember: true, ipAddress, additionalInformation, cancellationToken);
    }
  }
}
