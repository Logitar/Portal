using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class DeleteTemplateHandler : IRequestHandler<DeleteTemplate, Template>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplateHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(DeleteTemplate request, CancellationToken cancellationToken)
  {
    TemplateAggregate template = await _templateRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<TemplateAggregate>(request.Id);
    RealmAggregate? realm = await _realmRepository.LoadAsync(template, cancellationToken);
    Template output = await _templateQuerier.GetAsync(template, cancellationToken);

    if (realm?.PasswordRecoveryTemplateId == template.Id)
    {
      realm.SetPasswordRecoveryTemplate(_applicationContext.ActorId, template: null);
      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    template.Delete(_applicationContext.ActorId);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return output;
  }
}
