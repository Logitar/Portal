using FluentValidation;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserModel>
  {
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IValidator<Session> _sessionValidator;
    private readonly IUserContext _userContext;
    private readonly IUserQuerier _userQuerier;
    private readonly IUserValidator _userValidator;

    public DeleteUserCommandHandler(IRepository repository,
      ISessionQuerier sessionQuerier,
      IValidator<Session> sessionValidator,
      IUserContext userContext,
      IUserQuerier userQuerier,
      IUserValidator userValidator)
    {
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionValidator = sessionValidator;
      _userContext = userContext;
      _userQuerier = userQuerier;
      _userValidator = userValidator;
    }

    public async Task<UserModel> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      User user = await _repository.LoadAsync<User>(new AggregateId(request.Id), cancellationToken)
        ?? throw EntityNotFoundException.Typed<User>(request.Id);

      user.Delete(_userContext.ActorId);
      await _userValidator.ValidateAndThrowAsync(user, cancellationToken);

      IEnumerable<AggregateId> ids = (await _sessionQuerier.GetPagedAsync(realm: user.RealmId?.ToString(), userId: user.Id.ToString(), cancellationToken: cancellationToken))
        .Items
        .Select(x => new AggregateId(x.Id));
      IEnumerable<Session> sessions = await _repository.LoadAsync<Session>(ids, cancellationToken);
      foreach (Session session in sessions)
      {
        session.Delete(_userContext.ActorId);
        _sessionValidator.ValidateAndThrow(session);
      }

      List<AggregateRoot> aggregates = new(capacity: 1 + sessions.Count()) { user };
      aggregates.AddRange(sessions);
      await _repository.SaveAsync(aggregates, cancellationToken);

      return await _userQuerier.GetAsync(user.Id.ToString(), cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={user.Id}' could not be found.");
    }
  }
}
