using FluentValidation;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal class SetDefaultSenderCommandHandler : IRequestHandler<SetDefaultSenderCommand, SenderModel>
  {
    private readonly IRepository _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly IValidator<Sender> _senderValidator;
    private readonly IUserContext _userContext;

    public SetDefaultSenderCommandHandler(IRepository repository,
      ISenderQuerier senderQuerier,
      IValidator<Sender> senderValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _senderQuerier = senderQuerier;
      _senderValidator = senderValidator;
      _userContext = userContext;
    }

    public async Task<SenderModel> Handle(SetDefaultSenderCommand request, CancellationToken cancellationToken)
    {
      Sender sender = await _repository.LoadAsync<Sender>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(request.Id);

      Sender @default = await _repository.LoadDefaultSenderAsync(sender.RealmId, cancellationToken)
        ?? throw new InvalidOperationException($"The default sender from realm '{(sender.RealmId.HasValue ? $"Id={sender.RealmId.Value}" : "Portal")}' could not be found.");

      if (!sender.Equals(@default))
      {
        @default.SetDefault(_userContext.ActorId, isDefault: false);
        _senderValidator.ValidateAndThrow(sender);

        sender.SetDefault(_userContext.ActorId);
        _senderValidator.ValidateAndThrow(sender);

        await _repository.SaveAsync(@default, cancellationToken);
        await _repository.SaveAsync(sender, cancellationToken);
      }

      return await _senderQuerier.GetAsync(sender.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The sender 'Id={sender.Id}' could not be found.");
    }
  }
}
