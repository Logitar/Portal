using FluentValidation;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Realms.Commands
{
  internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, RealmModel>
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IValidator<Realm> _realmValidator;

    public CreateRealmCommandHandler(IRealmQuerier realmQuerier,
      IRepository repository,
      IUserContext userContext,
      IValidator<Realm> realmValidator)
    {
      _realmQuerier = realmQuerier;
      _repository = repository;
      _userContext = userContext;
      _realmValidator = realmValidator;
    }

    public async Task<RealmModel> Handle(CreateRealmCommand request, CancellationToken cancellationToken)
    {
      CreateRealmPayload payload = request.Payload;

      if (await _repository.LoadRealmByAliasOrIdAsync(payload.Alias, cancellationToken) != null)
      {
        throw new AliasAlreadyUsedException(payload.Alias, nameof(payload.Alias));
      }

      UsernameSettings usernameSettings = payload.UsernameSettings.GetUsernameSettings();
      PasswordSettings passwordSettings = payload.PasswordSettings.GetPasswordSettings();

      CultureInfo? defaultLocale = payload.DefaultLocale?.GetCultureInfo();
      Realm realm = new(_userContext.ActorId, payload.Alias, payload.JwtSecret,
        usernameSettings, passwordSettings,
        payload.DisplayName, payload.Description, defaultLocale, payload.Url,
        payload.RequireConfirmedAccount, payload.RequireUniqueEmail, payload.GoogleClientId);
      _realmValidator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return await _realmQuerier.GetAsync(realm.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The realm 'Id={realm.Id}' could not be found.");
    }
  }
}
