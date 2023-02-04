using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands
{
  internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SentMessagesModel>
  {
    private readonly IInternalMessageService _internalMessageService;

    public SendMessageCommandHandler(IInternalMessageService internalMessageService)
    {
      _internalMessageService = internalMessageService;
    }

    public async Task<SentMessagesModel> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
      return await _internalMessageService.SendAsync(request.Payload, cancellationToken);
    }
  }
}
