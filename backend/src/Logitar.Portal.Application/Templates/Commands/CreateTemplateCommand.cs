using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Templates.Validators;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal record CreateTemplateCommand(CreateTemplatePayload Payload) : Activity, IRequest<TemplateModel>;

internal class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, TemplateModel>
{
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public CreateTemplateCommandHandler(ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<TemplateModel> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
  {
    CreateTemplatePayload payload = command.Payload;
    new CreateTemplateValidator().ValidateAndThrow(payload);

    TemplateId templateId = TemplateId.NewId(command.TenantId);
    Template? template;
    if (payload.Id.HasValue)
    {
      templateId = new(command.TenantId, new EntityId(payload.Id.Value));
      template = await _templateRepository.LoadAsync(templateId, cancellationToken);
      if (template != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    ActorId actorId = command.ActorId;

    Identifier uniqueKey = new(payload.UniqueKey);
    Subject subject = new(payload.Subject);
    Content content = new(payload.Content);
    template = new(uniqueKey, subject, content, actorId, templateId)
    {
      DisplayName = DisplayName.TryCreate(payload.DisplayName),
      Description = Description.TryCreate(payload.Description)
    };
    template.Update(actorId);

    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);
  }
}
