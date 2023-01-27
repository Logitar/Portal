using FluentValidation;
using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands
{
  internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, RealmModel>
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly IValidator<Realm> _realmValidator;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;

    public DeleteRealmCommandHandler(IRealmQuerier realmQuerier,
      IValidator<Realm> realmValidator,
      IRepository repository,
      IUserContext userContext)
    {
      _realmQuerier = realmQuerier;
      _realmValidator = realmValidator;
      _repository = repository;
      _userContext = userContext;
    }

    public async Task<RealmModel> Handle(DeleteRealmCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadAsync<Realm>(new AggregateId(request.Id), cancellationToken)
        ?? throw EntityNotFoundException.Typed<Realm>(request.Id);

      // TODO(fpion): delete sessions & users
      // TODO(fpion): delete senders & templates
      // TODO(fpion): delete dictionaries

      realm.Delete(_userContext.ActorId);
      _realmValidator.ValidateAndThrow(realm);

      await _repository.SaveAsync(realm, cancellationToken);

      return await _realmQuerier.GetAsync(realm.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The realm 'Id={realm.Id}' could not be found.");
    }
  }
}
