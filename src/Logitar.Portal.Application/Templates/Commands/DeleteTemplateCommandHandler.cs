using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands;

internal class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, Template?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public DeleteTemplateCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository,
    ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template?> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
  {
    TemplateAggregate? template = await _templateRepository.LoadAsync(command.Id, cancellationToken);
    if (template == null)
    {
      return null;
    }
    RealmAggregate? realm = await _realmRepository.LoadAsync(template, cancellationToken);
    Template result = await _templateQuerier.ReadAsync(template, cancellationToken);

    template.Delete(_applicationContext.ActorId);

    if (realm?.PasswordRecoveryTemplateId == template.Id)
    {
      realm.RemovePasswordRecoveryTemplate();
      realm.Update(_applicationContext.ActorId);

      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    await _templateRepository.SaveAsync(template, cancellationToken);

    return result;
  }
}
