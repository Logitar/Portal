using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Realms.Queries;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Activities;

internal class ActivityPipeline : IActivityPipeline
{
  private readonly ICacheService _cacheService;
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;
  private readonly IContextParametersResolver _parametersResolver;

  public ActivityPipeline(ICacheService cacheService, ILoggingService loggingService, IMediator mediator, IContextParametersResolver parametersResolver)
  {
    _cacheService = cacheService;
    _loggingService = loggingService;
    _mediator = mediator;
    _parametersResolver = parametersResolver;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    IContextParameters parameters = _parametersResolver.Resolve();
    return await ExecuteAsync(request, parameters, cancellationToken);
  }
  public async Task<T> ExecuteAsync<T>(IRequest<T> request, IContextParameters parameters, CancellationToken cancellationToken)
  {
    try
    {
      if (request is IActivity activity)
      {
        ActivityContext context = await GetContextAsync(parameters, cancellationToken);
        activity.Contextualize(context);

        _loggingService.SetActivity(activity);
      }

      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      _loggingService.Report(exception);

      throw;
    }
  }

  private async Task<ActivityContext> GetContextAsync(IContextParameters parameters, CancellationToken cancellationToken)
  {
    ConfigurationModel configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
    RealmModel? realm = parameters.Realm;
    ApiKeyModel? apiKey = parameters.ApiKey;
    UserModel? user = parameters.User;
    SessionModel? session = parameters.Session;

    if (!string.IsNullOrWhiteSpace(parameters.RealmOverride))
    {
      realm = await ResolveRealmAsync(parameters.RealmOverride.Trim(), cancellationToken);
      _loggingService.SetRealm(realm);
    }

    if (!string.IsNullOrWhiteSpace(parameters.ImpersonifiedUser))
    {
      user = await ResolveUserAsync(parameters.ImpersonifiedUser.Trim(), configuration, realm, cancellationToken);
      _loggingService.SetUser(user);
    }

    return new ActivityContext(configuration, realm, apiKey, user, session);
  }
  private async Task<RealmModel> ResolveRealmAsync(string idOrUniqueSlug, CancellationToken cancellationToken)
  {
    bool isId = Guid.TryParse(idOrUniqueSlug, out Guid id);

    ReadRealmQuery query = new(isId ? id : null, idOrUniqueSlug);
    return await _mediator.Send(query, cancellationToken) ?? throw new RealmNotFoundException(idOrUniqueSlug);
  }
  private async Task<UserModel> ResolveUserAsync(string idOrUniqueNameOrCustomIdentifier, ConfigurationModel configuration, RealmModel? realm, CancellationToken cancellationToken)
  {
    bool isId = Guid.TryParse(idOrUniqueNameOrCustomIdentifier, out Guid id);
    CustomIdentifier? identifier = ParseCustomIdentifier(idOrUniqueNameOrCustomIdentifier);

    ReadUserQuery query = new(isId ? id : null, idOrUniqueNameOrCustomIdentifier, identifier);
    ActivityContext context = new(configuration, realm, ApiKey: null, User: null, Session: null);
    query.Contextualize(context);
    return await _mediator.Send(query, cancellationToken) ?? throw new ImpersonifiedUserNotFoundException(realm?.GetTenantId(), idOrUniqueNameOrCustomIdentifier);
  }
  private static CustomIdentifier? ParseCustomIdentifier(string value)
  {
    int index = value.IndexOf(':');
    return index < 0 ? null : new CustomIdentifier(value[..index], value[(index + 1)..]);
  }
}
