using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit;

internal class PopulateRequest : IPopulateRequest
{
  private readonly ICacheService _cacheService;
  private readonly IRealmService _realmService;
  private readonly IUserService _userService;

  public PopulateRequest(ICacheService cacheService, IRealmService realmService, IUserService userService)
  {
    _cacheService = cacheService;
    _realmService = realmService;
    _userService = userService;
  }

  public async Task ExecuteAsync<T>(ConsumeContext context, IRequest<T> request)
  {
    if (request is ApplicationRequest applicationRequest)
    {
      Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration was not found in the cache.");
      Actor actor = await ResolveActorAsync(context);
      Realm? realm = await ResolveRealmAsync(context);

      applicationRequest.Populate(actor, configuration, realm);
    }
  }

  private async Task<Actor> ResolveActorAsync(ConsumeContext context)
  {
    Actor actor = Actor.System;

    if (context.TryGetHeader(Contracts.Constants.Headers.User, out string? idOrUniqueName))
    {
      bool parsed = Guid.TryParse(idOrUniqueName.Trim(), out Guid id);
      User user = await _userService.ReadAsync(parsed ? id : null, idOrUniqueName, identifier: null, context.CancellationToken)
        ?? throw new InvalidOperationException($"The user '{idOrUniqueName}' could not be found.");
      actor = new Actor(user);
    }

    return actor;
  }

  private async Task<Realm?> ResolveRealmAsync(ConsumeContext context)
  {
    Realm? realm = null;

    if (context.TryGetHeader(Contracts.Constants.Headers.Realm, out string? idOrUniqueSlug))
    {
      bool parsed = Guid.TryParse(idOrUniqueSlug.Trim(), out Guid id);
      realm = await _realmService.ReadAsync(parsed ? id : null, idOrUniqueSlug, context.CancellationToken)
        ?? throw new InvalidOperationException($"The realm '{idOrUniqueSlug}' could not be found.");
    }

    return realm;
  }
}
