using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries
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
      return await _repository.LoadConfigurationAsync(cancellationToken) != null;
    }
  }
}
