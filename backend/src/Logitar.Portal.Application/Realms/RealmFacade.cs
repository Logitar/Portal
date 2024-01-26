using Logitar.Portal.Application.Realms.Commands;
using Logitar.Portal.Application.Realms.Queries;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms;

internal class RealmFacade : IRealmService
{
  private readonly IMediator _mediator;

  public RealmFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Realm> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateRealmCommand(payload), cancellationToken);
  }

  public async Task<Realm?> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteRealmCommand(id), cancellationToken);
  }

  public async Task<Realm?> ReadAsync(string? id, string? uniqueSlug, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadRealmQuery(id, uniqueSlug), cancellationToken);
  }

  public async Task<Realm?> ReplaceAsync(string id, ReplaceRealmPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceRealmCommand(id, payload, version), cancellationToken);
  }

  public async Task<Realm?> UpdateAsync(string id, UpdateRealmPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateRealmCommand(id, payload), cancellationToken);
  }
}
