using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Templates.Validators;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class ReplaceTemplateCommandHandler : IRequestHandler<ReplaceTemplateCommand, Template?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public ReplaceTemplateCommandHandler(IApplicationContext applicationContext,
    ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template?> Handle(ReplaceTemplateCommand command, CancellationToken cancellationToken)
  {
    ReplaceTemplatePayload payload = command.Payload;
    new ReplaceTemplateValidator().ValidateAndThrow(payload);

    TemplateAggregate? template = await _templateRepository.LoadAsync(command.Id, cancellationToken);
    if (template == null || template.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    TemplateAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _templateRepository.LoadAsync(template.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    UniqueKeyUnit uniqueKey = new(payload.UniqueKey);
    if (reference == null || uniqueKey != reference.UniqueKey)
    {
      template.SetUniqueKey(uniqueKey, actorId);
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      template.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      template.Description = description;
    }

    SubjectUnit subject = new(payload.Subject);
    if (reference == null || subject != reference.Subject)
    {
      template.Subject = subject;
    }
    ContentUnit content = new(payload.Content);
    if (reference == null || content != reference.Content)
    {
      template.Content = content;
    }

    template.Update(actorId);
    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return await _templateQuerier.ReadAsync(template, cancellationToken);
  }
}
