using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries
{
  internal class GetDefaultSenderQueryHandler : IRequestHandler<GetDefaultSenderQuery, SenderModel?>
  {
    private readonly IRepository _repository;
    private readonly ISenderQuerier _senderQuerier;

    public GetDefaultSenderQueryHandler(IRepository repository, ISenderQuerier senderQuerier)
    {
      _repository = repository;
      _senderQuerier = senderQuerier;
    }

    public async Task<SenderModel?> Handle(GetDefaultSenderQuery request, CancellationToken cancellationToken)
    {
      Realm? realm = request.Realm == null ? null
        : (await _repository.LoadRealmByAliasOrIdAsync(request.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(request.Realm));

      return await _senderQuerier.GetDefaultAsync(realm, cancellationToken);
    }
  }
}
