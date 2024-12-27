using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Templates.Validators;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record ReplaceTemplateCommand(Guid Id, ReplaceTemplatePayload Payload, long? Version) : Activity, IRequest<TemplateModel?>;

internal class ReplaceTemplateCommandHandler : IRequestHandler<ReplaceTemplateCommand, TemplateModel?>
{
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public ReplaceTemplateCommandHandler(ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<TemplateModel?> Handle(ReplaceTemplateCommand command, CancellationToken cancellationToken)
  {
    ReplaceTemplatePayload payload = command.Payload;
    new ReplaceTemplateValidator().ValidateAndThrow(payload);

    TemplateId templateId = new(command.TenantId, new EntityId(command.Id));
    Template? template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    if (template == null || template.TenantId != command.TenantId)
    {
      return null;
    }
    Template? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _templateRepository.LoadAsync(template.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    Identifier uniqueKey = new(payload.UniqueKey);
    if (reference == null || uniqueKey != reference.UniqueKey)
    {
      template.SetUniqueKey(uniqueKey, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      template.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      template.Description = description;
    }

    Subject subject = new(payload.Subject);
    if (reference == null || subject != reference.Subject)
    {
      template.Subject = subject;
    }
    Content content = new(payload.Content);
    if (reference == null || content != reference.Content)
    {
      template.Content = content;
    }

    template.Update(actorId);
    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);
  }
}
