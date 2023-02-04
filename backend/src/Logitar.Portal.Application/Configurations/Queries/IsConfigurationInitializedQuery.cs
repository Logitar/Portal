using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries
{
  internal record IsConfigurationInitializedQuery() : IRequest<bool>;
}
