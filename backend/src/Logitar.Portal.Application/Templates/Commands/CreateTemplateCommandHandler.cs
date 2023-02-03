using FluentValidation;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands
{
  internal class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, TemplateModel>
  {
    private readonly IRepository _repository;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IValidator<Template> _templateValidator;
    private readonly IUserContext _userContext;

    public CreateTemplateCommandHandler(IRepository repository,
      ITemplateQuerier templateQuerier,
      IValidator<Template> templateValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _templateQuerier = templateQuerier;
      _templateValidator = templateValidator;
      _userContext = userContext;
    }

    public async Task<TemplateModel> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
      CreateTemplatePayload payload = request.Payload;

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      if (await _repository.LoadTemplateByKeyAsync(payload.Key, realm, cancellationToken) != null)
      {
        throw new KeyAlreadyUsedException(payload.Key, nameof(payload.Key));
      }

      Template template = new(_userContext.ActorId, payload.Key, payload.Subject, payload.ContentType, payload.Contents, realm, payload.DisplayName, payload.Description);
      _templateValidator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return await _templateQuerier.GetAsync(template.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The template 'Id={template.Id}' could not be found.");
    }
  }
}
