﻿using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand>
  {
    private readonly IRepository _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly IUserContext _userContext;

    public DeleteSenderCommandHandler(IRepository repository,
      ISenderQuerier senderQuerier,
      IUserContext userContext)
    {
      _repository = repository;
      _senderQuerier = senderQuerier;
      _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteSenderCommand request, CancellationToken cancellationToken)
    {
      Sender sender = await _repository.LoadAsync<Sender>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(request.Id);

      if (sender.IsDefault)
      {
        ListModel<SenderModel> senders = await _senderQuerier.GetPagedAsync(realm: sender.RealmId?.Value, cancellationToken: cancellationToken);
        if (senders.Total > 1)
        {
          throw new CannotDeleteDefaultSenderException(sender);
        }
      }

      if (sender.RealmId.HasValue)
      {
        Realm realm = await _repository.LoadAsync<Realm>(sender.RealmId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The realm 'Id={sender.RealmId}' could not be found.");

        if (realm.PasswordRecoverySenderId == sender.Id)
        {
          throw new CannotDeletePasswordRecoverySenderException(sender, realm);
        }
      }

      sender.Delete(_userContext.ActorId);
      await _repository.SaveAsync(sender, cancellationToken);

      return Unit.Value;
    }
  }
}
