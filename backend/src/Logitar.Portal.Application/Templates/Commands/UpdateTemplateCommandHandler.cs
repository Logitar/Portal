using FluentValidation;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands
{
  internal class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateModel>
  {
    private readonly IRepository _repository;
    private readonly ITemplateQuerier _templateQuerier;
    private readonly IValidator<Template> _templateValidator;
    private readonly IUserContext _userContext;

    public UpdateTemplateCommandHandler(IRepository repository,
      ITemplateQuerier templateQuerier,
      IValidator<Template> templateValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _templateQuerier = templateQuerier;
      _templateValidator = templateValidator;
      _userContext = userContext;
    }

    public async Task<TemplateModel> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
      Template template = await _repository.LoadAsync<Template>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(request.Id);

      UpdateTemplatePayload payload = request.Payload;

      template.Update(_userContext.ActorId, payload.Subject, payload.ContentType, payload.Contents, payload.DisplayName, payload.Description);
      _templateValidator.ValidateAndThrow(template);

      await _repository.SaveAsync(template, cancellationToken);

      return await _templateQuerier.GetAsync(template.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The template 'Id={template.Id}' could not be found.");
    }
  }
}
