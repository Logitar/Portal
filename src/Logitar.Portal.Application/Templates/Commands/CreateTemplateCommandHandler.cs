using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, Template>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public CreateTemplateCommandHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
  {
    CreateTemplatePayload payload = command.Payload;

    RealmAggregate? realm = null;
    if (payload.Realm != null)
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }
    string? tenantId = realm?.Id.Value;
    IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;

    if (await _templateRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException<TemplateAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
    }

    TemplateAggregate template = new(uniqueNameSettings, payload.UniqueName, payload.Subject,
      payload.ContentType, payload.Contents, tenantId, _applicationContext.ActorId)
    {
      DisplayName = payload.DisplayName,
      Description = payload.Description
    };

    template.Update(_applicationContext.ActorId);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.ReadAsync(template, cancellationToken);
  }
}
