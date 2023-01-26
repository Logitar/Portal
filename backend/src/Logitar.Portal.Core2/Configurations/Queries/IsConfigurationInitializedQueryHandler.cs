using MediatR;

namespace Logitar.Portal.Core2.Configurations.Queries
{
  internal class IsConfigurationInitializedQueryHandler : IRequestHandler<IsConfigurationInitializedQuery, bool>
  {
    private readonly IRepository _repository;

    public IsConfigurationInitializedQueryHandler(IRepository repository)
    {
      _repository = repository;
    }

    public async Task<bool> Handle(IsConfigurationInitializedQuery request, CancellationToken cancellationToken)
    {
      Configuration? configuration = await _repository.LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken);

      return configuration != null;
    }
  }
}
