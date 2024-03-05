using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Templates.Validators;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, Template>
{
  private readonly ITemplateManager _templateManager;
  private readonly ITemplateQuerier _templateQuerier;

  public CreateTemplateCommandHandler(ITemplateManager templateManager, ITemplateQuerier templateQuerier)
  {
    _templateManager = templateManager;
    _templateQuerier = templateQuerier;
  }

  public async Task<Template> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
  {
    CreateTemplatePayload payload = command.Payload;
    new CreateTemplateValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    UniqueKeyUnit uniqueKey = new(payload.UniqueKey);
    SubjectUnit subject = new(payload.Subject);
    ContentUnit content = new(payload.Content);
    TemplateAggregate template = new(uniqueKey, subject, content, command.TenantId, actorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    template.Update(actorId);

    await _templateManager.SaveAsync(template, actorId, cancellationToken);

    return await _templateQuerier.ReadAsync(command.Realm, template, cancellationToken);
  }
}
