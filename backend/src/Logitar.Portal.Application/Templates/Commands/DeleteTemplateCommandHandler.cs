using Logitar.Portal.Domain.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Commands
{
  internal class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand>
  {
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteTemplateCommandHandler(IRepository repository, IUserContext userContext)
    {
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {
      Template template = await _repository.LoadAsync<Template>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Template>(request.Id);

      // TODO(fpion): what if is set as PasswordRecoverTemplate in its realm?

      template.Delete(_userContext.ActorId);

      await _repository.SaveAsync(template, cancellationToken);

      return Unit.Value;
    }
  }
}
