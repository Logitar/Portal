using FluentValidation;
using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands
{
  internal class SignOutSessionCommandHandler : IRequestHandler<SignOutSessionCommand, SessionModel>
  {
    private readonly IRepository _repository;
    private readonly ISessionQuerier _sessionQuerier;
    private readonly IValidator<Session> _sessionValidator;

    public SignOutSessionCommandHandler(IRepository repository,
      ISessionQuerier sessionQuerier,
      IValidator<Session> sessionValidator)
    {
      _repository = repository;
      _sessionQuerier = sessionQuerier;
      _sessionValidator = sessionValidator;
    }

    public async Task<SessionModel> Handle(SignOutSessionCommand request, CancellationToken cancellationToken)
    {
      Session session = await _repository.LoadAsync<Session>(new AggregateId(request.Id), cancellationToken)
        ?? throw EntityNotFoundException.Typed<Session>(request.Id);

      session.SignOut();
      _sessionValidator.ValidateAndThrow(session);

      await _repository.SaveAsync(session, cancellationToken);

      return await _sessionQuerier.GetAsync(request.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The session 'Id={request.Id}' could not be found.");
    }
  }
}
