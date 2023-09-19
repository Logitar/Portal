using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class ReplaceTemplateCommandHandler : IRequestHandler<ReplaceTemplateCommand, Template?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public ReplaceTemplateCommandHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template?> Handle(ReplaceTemplateCommand command, CancellationToken cancellationToken)
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

    TemplateAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _templateRepository.LoadAsync(template.Id, command.Version.Value, cancellationToken);
    }

    ReplaceTemplatePayload payload = command.Payload;

    if (reference == null || (payload.UniqueName.Trim() != reference.UniqueName))
    {
      TemplateAggregate? other = await _templateRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
      if (other?.Equals(template) == false)
      {
        throw new UniqueNameAlreadyUsedException<TemplateAggregate>(tenantId, payload.UniqueName, nameof(payload.UniqueName));
      }

      IUniqueNameSettings uniqueNameSettings = realm?.UniqueNameSettings ?? _applicationContext.Configuration.UniqueNameSettings;
      template.SetUniqueName(uniqueNameSettings, payload.UniqueName);
    }
    if (reference == null || (payload.DisplayName?.CleanTrim() != reference.DisplayName))
    {
      template.DisplayName = payload.DisplayName;
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description))
    {
      template.Description = payload.Description;
    }

    if (reference == null || (payload.Subject.Trim() != reference.Subject))
    {
      template.Subject = payload.Subject;
    }
    if (reference == null || (payload.ContentType.Trim() != reference.ContentType))
    {
      template.ContentType = payload.ContentType;
    }
    if (reference == null || (payload.Contents.Trim() != reference.Contents))
    {
      template.Contents = payload.Contents;
    }

    template.Update(_applicationContext.ActorId);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.ReadAsync(template, cancellationToken);
  }
}
