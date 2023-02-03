using FluentValidation;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal class UpdateSenderCommandHandler : IRequestHandler<UpdateSenderCommand, SenderModel>
  {
    private readonly IRepository _repository;
    private readonly ISenderQuerier _senderQuerier;
    private readonly IValidator<Sender> _senderValidator;
    private readonly IUserContext _userContext;

    public UpdateSenderCommandHandler(IRepository repository,
      ISenderQuerier senderQuerier,
      IValidator<Sender> senderValidator,
      IUserContext userContext)
    {
      _repository = repository;
      _senderQuerier = senderQuerier;
      _senderValidator = senderValidator;
      _userContext = userContext;
    }

    public async Task<SenderModel> Handle(UpdateSenderCommand request, CancellationToken cancellationToken)
    {
      Sender sender = await _repository.LoadAsync<Sender>(request.Id, cancellationToken)
        ?? throw new EntityNotFoundException<Sender>(request.Id);

      UpdateSenderPayload payload = request.Payload;

      Dictionary<string, string?>? settings = payload.Settings?.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Last().Value);
      sender.Update(_userContext.ActorId, payload.EmailAddress, payload.DisplayName, settings);
      _senderValidator.ValidateAndThrow(sender);

      await _repository.SaveAsync(sender, cancellationToken);

      return await _senderQuerier.GetAsync(sender.Id, cancellationToken)
        ?? throw new InvalidOperationException($"The sender 'Id={sender.Id}' could not be found.");
    }
  }
}
