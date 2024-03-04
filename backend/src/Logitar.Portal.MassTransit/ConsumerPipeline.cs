using Logitar.Portal.Application;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Realms.Queries;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.MassTransit.Settings;
using MassTransit;
using MediatR;
using System.Net;
using System.Text.Json;

namespace Logitar.Portal.MassTransit;

internal class ConsumerPipeline : IConsumerPipeline
{
  private const string OperationType = nameof(MassTransit);

  private readonly ICacheService _cacheService;
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;
  private readonly MassTransitSettings _settings;

  public ConsumerPipeline(ICacheService cacheService, ILoggingService loggingService, IMediator mediator, MassTransitSettings settings)
  {
    _cacheService = cacheService;
    _loggingService = loggingService;
    _mediator = mediator;
    _settings = settings;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, Type consumerType, ConsumeContext context)
  {
    CancellationToken cancellationToken = context.CancellationToken;

    string? correlationId = context.CorrelationId?.ToString();
    string? method = _settings.TransportBroker?.ToString() ?? OperationType;
    string? destination = context.DestinationAddress?.ToString();
    string? source = context.SourceAddress?.ToString();
    string? additionalInformation = JsonSerializer.Serialize(context.Headers, context.Headers.GetType());
    _loggingService.Open(correlationId, method, destination, source, additionalInformation);

    Operation operation = new(OperationType, consumerType.Name);
    _loggingService.SetOperation(operation);

    int statusCode = (int)HttpStatusCode.OK;
    try
    {
      if (request is ApplicationRequest applicationRequest)
      {
        await ContextualizeAsync(applicationRequest, context);
      }

      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      statusCode = (int)HttpStatusCode.InternalServerError;

      _loggingService.Report(exception);

      throw;
    }
    finally
    {
      await _loggingService.CloseAndSaveAsync(statusCode, cancellationToken);
    }
  }

  private async Task ContextualizeAsync(ApplicationRequest request, ConsumeContext context)
  {
    Configuration configuration = _cacheService.Configuration ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
    Realm? realm = await ResolveRealmAsync(context);
    User? user = await ResolveUserAsync(context, configuration, realm);

    ApplicationContext applicationContext = new(configuration, realm, ApiKey: null, user, Session: null);
    request.Contextualize(applicationContext);

    if (realm != null)
    {
      _loggingService.SetRealm(realm);
    }
    if (user != null)
    {
      _loggingService.SetUser(user);
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
