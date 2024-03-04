using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Realms.Queries;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit;

internal class PopulateRequest : IPopulateRequest
{
  private readonly ICacheService _cacheService;
  private readonly IMediator _mediator;

  public PopulateRequest(ICacheService cacheService, IMediator mediator)
  {
    _cacheService = cacheService;
    _mediator = mediator;
  }

  public async Task ExecuteAsync<T>(ConsumeContext context, IRequest<T> request)
  {
    if (request is ApplicationRequest applicationRequest)
    {
      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
      Realm? realm = await ResolveRealmAsync(context);
      User? user = await ResolveUserAsync(context, configuration, realm);

      ApplicationContext applicationContext = new(configuration, realm, ApiKey: null, user, Session: null);
      applicationRequest.Contextualize(applicationContext);
    }
  }

  private async Task<Realm?> ResolveRealmAsync(ConsumeContext context)
  {
    Realm? realm = null;

    if (context.TryGetHeader(Contracts.Constants.Headers.Realm, out string? idOrUniqueSlug))
    {
      bool parsed = Guid.TryParse(idOrUniqueSlug.Trim(), out Guid id);
      realm = await _mediator.Send(new ReadRealmQuery(parsed ? id : null, idOrUniqueSlug), context.CancellationToken)
        ?? throw new InvalidOperationException($"The realm '{idOrUniqueSlug}' could not be found.");
    }

    return realm;
  }

  private async Task<User?> ResolveUserAsync(ConsumeContext context, Configuration configuration, Realm? realm)
  {
    User? user = null;

    if (context.TryGetHeader(Contracts.Constants.Headers.User, out string? idOrUniqueName))
    {
      bool parsed = Guid.TryParse(idOrUniqueName.Trim(), out Guid id);

      ReadUserQuery query = new(parsed ? id : null, idOrUniqueName, Identifier: null);
      ApplicationContext applicationContext = new(configuration, realm, ApiKey: null, User: null, Session: null);
      query.Contextualize(applicationContext);

      user = await _mediator.Send(query, context.CancellationToken)
        ?? throw new InvalidOperationException($"The user '{idOrUniqueName}' could not be found");
    }

    return user;
  }
}
