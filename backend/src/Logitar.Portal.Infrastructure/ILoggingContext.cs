using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;

namespace Logitar.Portal.Infrastructure
{
  internal interface ILoggingContext
  {
    void AddError(Error error);
    void AddError(Exception exception);
    void AddEvents(IEnumerable<EventEntity> @event);
    Task CompleteAsync(Configuration configuration, int statusCode, ActorModel actor, ApiKeyModel? apiKey = null, UserModel? user = null, SessionModel? session = null, CancellationToken cancellationToken = default);
    void Start(string traceIdentifier, string method, string url, string? ipAddress = null, string? additionalInformation = null);
    void SetRequest<T>(IRequest<T> request);
  }
}
