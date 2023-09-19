using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, Template?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public UpdateTemplateCommandHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template?> Handle(UpdateTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateAggregate? template = await _templateRepository.LoadAsync(command.Id, cancellationToken);
    if (template == null)
    {
      return null;
    }

    RealmAggregate? realm = null;
    if (template.TenantId != null)
    {
      realm = await _realmRepository.LoadAsync(template, cancellationToken);
    }
    string? tenantId = realm?.Id.Value;

    UpdateTemplatePayload payload = command.Payload;

    if (payload.UniqueName != null)
    {
      TemplateAggregate? other = await _templateRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(template) == false)
      {
        throw new UniqueNameAlreadyUsedException<TemplateAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      template.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (payload.DisplayName != null)
    {
      template.DisplayName = payload.DisplayName.Value;
    }
    if (payload.Description != null)
    {
      template.Description = payload.Description.Value;
    }

    if (payload.Subject != null)
    {
      template.Subject = payload.Subject;
    }
    if (payload.ContentType != null)
    {
      template.ContentType = payload.ContentType;
    }
    if (payload.Contents != null)
    {
      template.Contents = payload.Contents;
    }

    template.Update(_applicationContext.ActorId);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.ReadAsync(template, cancellationToken);
  }
}
