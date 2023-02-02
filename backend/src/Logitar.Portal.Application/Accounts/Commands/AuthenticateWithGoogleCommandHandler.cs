using Google.Apis.Auth;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Sessions;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class AuthenticateWithGoogleCommandHandler : IRequestHandler<AuthenticateWithGoogleCommand, SessionModel>
  {
    private readonly IRealmRepository _realmRepository;
    private readonly ISignInService _signInService;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserValidator _userValidator;

    public AuthenticateWithGoogleCommandHandler(IRealmRepository realmRepository,
      ISignInService signInService,
      IUserContext userContext,
      IUserRepository userRepository,
      IUserValidator userValidator)
    {
      _realmRepository = realmRepository;
      _signInService = signInService;
      _userContext = userContext;
      _userRepository = userRepository;
      _userValidator = userValidator;
    }

    public async Task<SessionModel> Handle(AuthenticateWithGoogleCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _realmRepository.LoadByAliasOrIdAsync(request.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(request.Realm);

      if (realm.GoogleClientId == null)
      {
        throw new GoogleClientIdRequiredException(realm);
      }

      AuthenticateWithGooglePayload payload = request.Payload;

      GoogleJsonWebSignature.ValidationSettings validationSettings = new()
      {
        Audience = new string[] { realm.GoogleClientId }
      };
      GoogleJsonWebSignature.Payload googlePayload = await GoogleJsonWebSignature.ValidateAsync(payload.Credential, validationSettings);

      User? user = await _userRepository.LoadByExternalProviderAsync(realm, ExternalProviders.Google, googlePayload.Subject, cancellationToken);
      if (user == null)
      {
        user = await _userRepository.LoadByUsernameAsync(googlePayload.Email, realm, cancellationToken);
        if (user == null)
        {
          if (realm.RequireUniqueEmail)
          {
            user = (await _userRepository.LoadByEmailAsync(googlePayload.Email, realm, cancellationToken))
              .SingleOrDefault();
          }

          if (user == null)
          {
            string? localeName = payload.IgnoreProviderLocale ? payload.Locale : (googlePayload.Locale ?? payload.Locale);
            CultureInfo? locale = localeName == null ? null : CultureInfo.GetCultureInfo(localeName);
            user = new(_userContext.ActorId, googlePayload.Email, realm, googlePayload.Email, googlePayload.EmailVerified,
              googlePayload.GivenName, googlePayload.FamilyName, locale, googlePayload.Picture);
            _userValidator.ValidateAndThrow(user, realm.UsernameSettings);

            await _userRepository.SaveAsync(user, cancellationToken);
          }
        }

        user.AddExternalProvider(_userContext.ActorId, ExternalProviders.Google, googlePayload.Subject, ExternalProviders.Google);
        _userValidator.ValidateAndThrow(user, realm.UsernameSettings);

        await _userRepository.SaveAsync(user, cancellationToken);
      }

      return await _signInService.SignInAsync(user, realm, remember: true,
        request.IpAddress, request.AdditionalInformation, cancellationToken);
    }
  }
}
