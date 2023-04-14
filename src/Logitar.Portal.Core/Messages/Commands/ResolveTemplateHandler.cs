using Logitar.Portal.Core.Templates;
using MediatR;

namespace Logitar.Portal.Core.Messages.Commands;

internal class ResolveTemplateHandler : IRequestHandler<ResolveTemplate, TemplateAggregate>
{
  private readonly ITemplateRepository _templateRepository;

  public ResolveTemplateHandler(ITemplateRepository templateRepository)
  {
    _templateRepository = templateRepository;
  }

  public async Task<TemplateAggregate> Handle(ResolveTemplate request, CancellationToken cancellationToken)
  {
    TemplateAggregate? template = null;
    if (Guid.TryParse(request.IdOrUniqueName, out Guid templateId))
    {
      template = await _templateRepository.LoadAsync(templateId, cancellationToken);
    }

    template ??= await _templateRepository.LoadByUniqueNameAsync(request.Realm, request.IdOrUniqueName, cancellationToken);

    if (template == null)
    {
      throw new AggregateNotFoundException<TemplateAggregate>(request.IdOrUniqueName, request.ParamName);
    }
    else if (request.Realm?.Id != template.RealmId)
    {
      throw new TemplateNotInRealmException(template, request.Realm, request.ParamName);
    }

    return template;
  }
}
