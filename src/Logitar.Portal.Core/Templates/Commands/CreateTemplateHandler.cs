using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Templates.Commands;

internal class CreateTemplateHandler : IRequestHandler<CreateTemplate, Template>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public CreateTemplateHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(CreateTemplate request, CancellationToken cancellationToken)
  {
    CreateTemplateInput input = request.Input;

    RealmAggregate? realm = await LoadRealmAsync(input, cancellationToken);

    string uniqueName = input.Key.Trim();
    if (await _templateRepository.LoadByUniqueNameAsync(realm, uniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(uniqueName, nameof(input.Key));
    }

    TemplateAggregate template = new(_applicationContext.ActorId, realm, uniqueName, input.Subject,
      input.ContentType, input.Contents, input.DisplayName, input.Description);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.GetAsync(template, cancellationToken);
  }

  private async Task<RealmAggregate?> LoadRealmAsync(CreateTemplateInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
  }
}
