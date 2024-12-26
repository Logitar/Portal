using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Templates.Validators;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record UpdateTemplateCommand(Guid Id, UpdateTemplatePayload Payload) : Activity, IRequest<TemplateModel?>;

internal class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateModel?>
{
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public UpdateTemplateCommandHandler(ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<TemplateModel?> Handle(UpdateTemplateCommand command, CancellationToken cancellationToken)
  {
    UpdateTemplatePayload payload = command.Payload;
    new UpdateTemplateValidator().ValidateAndThrow(payload);

    Template? template = await _templateRepository.LoadAsync(command.Id, cancellationToken);
    if (template == null || template.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    UniqueKey? uniqueKey = UniqueKey.TryCreate(payload.UniqueKey);
    if (uniqueKey != null)
    {
      template.SetUniqueKey(uniqueKey, actorId);
    }
    if (payload.DisplayName != null)
    {
      template.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      template.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    Subject? subject = Subject.TryCreate(payload.Subject);
    if (subject != null)
    {
      template.Subject = subject;
    }
    if (payload.Content != null)
    {
      template.Content = new Content(payload.Content);
    }

    template.Update(actorId);
    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);
  }
}
