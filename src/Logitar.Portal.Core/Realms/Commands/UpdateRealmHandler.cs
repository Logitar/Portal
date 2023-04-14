using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Realms.Commands;

internal class UpdateRealmHandler : IRequestHandler<UpdateRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  public UpdateRealmHandler(ICurrentActor currentActor,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository,
    ISenderRepository senderRepository,
    ITemplateRepository templateRepository)
  {
    _currentActor = currentActor;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _senderRepository = senderRepository;
    _templateRepository = templateRepository;
  }

  public async Task<Realm> Handle(UpdateRealm request, CancellationToken cancellationToken)
  {
    RealmAggregate realm = await _realmRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Id);
    if (realm.UniqueName.ToLower() == RealmAggregate.PortalUniqueName)
    {
      throw new CannotManagePortalRealmException(_currentActor.Id);
    }

    UpdateRealmInput input = request.Input;

    SenderAggregate? passwordRecoverySender = null;
    if (input.PasswordRecoverySender.HasValue)
    {
      passwordRecoverySender = await _senderRepository.LoadAsync(input.PasswordRecoverySender.Value, cancellationToken)
        ?? throw new AggregateNotFoundException<SenderAggregate>(input.PasswordRecoverySender.Value, nameof(input.PasswordRecoverySender));
    }

    TemplateAggregate? passwordRecoveryTemplate = null;
    if (input.PasswordRecoveryTemplate != null)
    {
      passwordRecoveryTemplate = (Guid.TryParse(input.PasswordRecoveryTemplate, out Guid templateId)
        ? await _templateRepository.LoadAsync(templateId, cancellationToken)
        : await _templateRepository.LoadByUniqueNameAsync(realm, input.PasswordRecoveryTemplate, cancellationToken)
      ) ?? throw new AggregateNotFoundException<TemplateAggregate>(input.PasswordRecoveryTemplate, nameof(input.PasswordRecoveryTemplate));
    }

    CultureInfo? defaultLocale = input.DefaultLocale?.GetCultureInfo(nameof(input.DefaultLocale));
    Uri? url = input.Url?.GetUri(nameof(input.Url));
    ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);

    realm.Update(_currentActor.Id, input.DisplayName, input.Description,
      defaultLocale, input.Secret, url,
      input.RequireConfirmedAccount, input.RequireUniqueEmail, usernameSettings, passwordSettings,
      input.ClaimMappings?.ToDictionary(), input.CustomAttributes?.ToDictionary());

    realm.SetPasswordRecoverySender(_currentActor.Id, passwordRecoverySender);
    realm.SetPasswordRecoveryTemplate(_currentActor.Id, passwordRecoveryTemplate);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return await _realmQuerier.GetAsync(realm, cancellationToken);
  }
}
