using Logitar.Portal.v2.Contracts.Templates;
using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Commands;

internal class CreateTemplateHandler : IRequestHandler<CreateTemplate, Template>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly ITemplateQuerier _templateQuerier;
  private readonly ITemplateRepository _templateRepository;

  public CreateTemplateHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _templateQuerier = templateQuerier;
    _templateRepository = templateRepository;
  }

  public async Task<Template> Handle(CreateTemplate request, CancellationToken cancellationToken)
  {
    CreateTemplateInput input = request.Input;

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    string uniqueName = input.Key.Trim();
    if (await _templateRepository.LoadByUniqueNameAsync(realm, uniqueName, cancellationToken) != null)
    {
      throw new UniqueNameAlreadyUsedException(uniqueName, nameof(input.Key));
    }

    TemplateAggregate template = new(_currentActor.Id, realm, uniqueName, input.Subject,
      input.ContentType, input.Contents, input.DisplayName, input.Description);

    await _templateRepository.SaveAsync(template, cancellationToken);

    return await _templateQuerier.GetAsync(template, cancellationToken);
  }
}
