using Logitar.Portal.Domain.Realms;
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

      if (template.RealmId.HasValue)
      {
        Realm realm = await _repository.LoadAsync<Realm>(template.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={template.RealmId}' could not be found.");

        if (realm.PasswordRecoveryTemplateId == template.Id)
        {
          throw new CannotDeletePasswordRecoveryTemplateException(template, realm);
        }
      }

      template.Delete(_userContext.ActorId);

      await _repository.SaveAsync(template, cancellationToken);

      return Unit.Value;
    }
  }
}
