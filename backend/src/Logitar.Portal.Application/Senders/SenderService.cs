using Logitar.Portal.Application.Senders.Commands;
using Logitar.Portal.Application.Senders.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Application.Senders
{
  internal class SenderService : ISenderService
  {
    private readonly IRequestPipeline _requestPipeline;

    public SenderService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<SenderModel> CreateAsync(CreateSenderPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateSenderCommand(payload), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new DeleteSenderCommand(id), cancellationToken);
    }

    public async Task<SenderModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetSenderQuery(id), cancellationToken);
    }

    public async Task<ListModel<SenderModel>> GetAsync(ProviderType? provider, string? realm, string? search,
      SenderSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetSendersQuery(provider, realm, search,
        sort, isDescending,
        index, count), cancellationToken);
    }

    public async Task<SenderModel?> GetDefaultAsync(string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetDefaultSenderQuery(realm), cancellationToken);
    }

    public async Task<SenderModel> SetDefaultAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SetDefaultSenderCommand(id), cancellationToken);
    }

    public async Task<SenderModel> UpdateAsync(string id, UpdateSenderPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateSenderCommand(id, payload), cancellationToken);
    }
  }
}
