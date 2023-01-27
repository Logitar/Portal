using FluentValidation;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Realms.Commands
{
  internal class CreateRealmCommandHandler : IRequestHandler<CreateRealmCommand, RealmModel>
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly IValidator<Realm> _realmValidator;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public CreateRealmCommandHandler(IRealmQuerier realmQuerier,
      IValidator<Realm> realmValidator,
      IRepository repository,
      IUserContext userContext)
    {
      _realmQuerier = realmQuerier;
      _realmValidator = realmValidator;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<RealmModel> Handle(CreateRealmCommand request, CancellationToken cancellationToken)
    {
      CreateRealmPayload payload = request.Payload;

      if (await _realmQuerier.GetAsync(payload.Alias, cancellationToken) != null)
      {
        throw new AliasAlreadyUsedException(payload.Alias, nameof(payload.Alias));
      }

      CultureInfo? defaultLocale = payload.DefaultLocale == null ? null : CultureInfo.GetCultureInfo(payload.DefaultLocale);
      Realm realm = new(payload.Alias, payload.DisplayName, payload.Description, defaultLocale, payload.Url,
        payload.RequireConfirmedAccount, payload.RequireUniqueEmail, payload.UsernameSettings, payload.PasswordSettings,
        payload.GoogleClientId, _userContext.ActorId);
      _realmValidator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return await _realmQuerier.GetAsync(realm.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The realm 'Id={realm.Id}' could not be found.");
    }
  }
}
