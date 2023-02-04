using FluentValidation;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal class CreateSenderCommandHandler : IRequestHandler<CreateSenderCommand, SenderModel>
  {
    private readonly IRepository _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly IValidator<Sender> _senderValidator;
    private readonly IUserContext _userContext;

    public CreateSenderCommandHandler(IRepository repository,
      ISenderQuerier senderQuerier,
      IValidator<Sender> senderValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _senderQuerier = senderQuerier;
      _senderValidator = senderValidator;
      _userContext = userContext;
    }

    public async Task<SenderModel> Handle(CreateSenderCommand request, CancellationToken cancellationToken)
    {
      CreateSenderPayload payload = request.Payload;

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      bool isDefault = await _senderQuerier.GetDefaultAsync(realm, cancellationToken) == null;

      Dictionary<string, string?>? settings = payload.Settings?.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
      Sender sender = new(_userContext.ActorId, payload.EmailAddress, payload.Provider, realm, isDefault, payload.DisplayName, settings);
      _senderValidator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return await _senderQuerier.GetAsync(sender.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The sender 'Id={sender.Id}' could not be found.");
    }
  }
}
