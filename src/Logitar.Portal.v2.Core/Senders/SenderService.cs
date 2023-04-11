using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.Core.Senders.Commands;
using Logitar.Portal.v2.Core.Senders.Queries;

namespace Logitar.Portal.v2.Core.Senders;

internal class SenderService : ISenderService
{
  private readonly IRequestPipeline _pipeline;

  public SenderService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<Sender> CreateAsync(CreateSenderInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateSender(input), cancellationToken);
  }

  public async Task<Sender> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteSender(id), cancellationToken);
  }

  public async Task<Sender?> GetAsync(Guid? id, string? defaultInRealm, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetSender(id, defaultInRealm), cancellationToken);
  }

  public async Task<PagedList<Sender>> GetAsync(ProviderType? provider, string? realm, string? search,
    SenderSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new GetSenders(provider, realm, search,
      sort, isDescending, skip, limit), cancellationToken);
  }

  public async Task<Sender> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SetDefaultSender(id), cancellationToken);
  }

  public async Task<Sender> UpdateAsync(Guid id, UpdateSenderInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateSender(id, input), cancellationToken);
  }
}
