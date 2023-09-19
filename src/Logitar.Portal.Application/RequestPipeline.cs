using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Users.Commands;
using MediatR;

namespace Logitar.Portal.Application;

internal class RequestPipeline : IRequestPipeline
{
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;

  public RequestPipeline(ILoggingService loggingService, IMediator mediator)
  {
    _loggingService = loggingService;
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    LogActivity(request);

    try
    {
      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      _loggingService.AddException(exception);

      throw;
    }
  }

  private void LogActivity<T>(IRequest<T> request)
  {
    switch (request)
    {
      case AuthenticateApiKeyCommand authenticateApiKeyCommand:
        authenticateApiKeyCommand = new(Mask(authenticateApiKeyCommand.XApiKey));
        _loggingService.SetActivity(authenticateApiKeyCommand);
        break;
      case AuthenticateUserCommand authenticateUserCommand:
        authenticateUserCommand = DeepClone(authenticateUserCommand);
        authenticateUserCommand.Payload.Password = Mask(authenticateUserCommand.Payload.Password);
        _loggingService.SetActivity(authenticateUserCommand);
        break;
      case CreateUserCommand createUserCommand:
        createUserCommand = DeepClone(createUserCommand);
        if (createUserCommand.Payload.Password != null)
        {
          createUserCommand.Payload.Password = Mask(createUserCommand.Payload.Password);
        }
        _loggingService.SetActivity(createUserCommand);
        break;
      case InitializeConfigurationCommand initializeConfiguration:
        initializeConfiguration = DeepClone(initializeConfiguration);
        initializeConfiguration.Payload.User.Password = Mask(initializeConfiguration.Payload.User.Password);
        _loggingService.SetActivity(initializeConfiguration);
        break;
      case ReplaceUserCommand replaceUserCommand:
        replaceUserCommand = DeepClone(replaceUserCommand);
        if (replaceUserCommand.Payload.Password != null)
        {
          replaceUserCommand.Payload.Password = Mask(replaceUserCommand.Payload.Password);
        }
        _loggingService.SetActivity(replaceUserCommand);
        break;
      case ResetPasswordCommand resetPasswordCommand:
        resetPasswordCommand = DeepClone(resetPasswordCommand);
        if (resetPasswordCommand.Payload.Password != null)
        {
          resetPasswordCommand.Payload.Password = Mask(resetPasswordCommand.Payload.Password);
        }
        _loggingService.SetActivity(resetPasswordCommand);
        break;
      case SignInCommand signInCommand:
        signInCommand = DeepClone(signInCommand);
        signInCommand.Payload.Password = Mask(signInCommand.Payload.Password);
        _loggingService.SetActivity(signInCommand);
        break;
      case UpdateUserCommand updateUserCommand:
        updateUserCommand = DeepClone(updateUserCommand);
        if (updateUserCommand.Payload.Password != null)
        {
          if (updateUserCommand.Payload.Password.CurrentPassword != null)
          {
            updateUserCommand.Payload.Password.CurrentPassword = Mask(updateUserCommand.Payload.Password.CurrentPassword);
          }
          updateUserCommand.Payload.Password.NewPassword = Mask(updateUserCommand.Payload.Password.NewPassword);
        }
        _loggingService.SetActivity(updateUserCommand);
        break;
      default:
        _loggingService.SetActivity(request);
        break;
    }
  }

  private static T DeepClone<T>(T value)
  {
    Type type = value?.GetType() ?? throw new ArgumentNullException(nameof(value));

    string json = JsonSerializer.Serialize(value, type);

    return (T?)JsonSerializer.Deserialize(json, type)
      ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }

  private static string Mask(string s) => new(s.Select(c => '*').ToArray());
}
