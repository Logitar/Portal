using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Users.Commands;

namespace Logitar.Portal.Application.Logging;

internal static class RequestExtensions
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static RequestExtensions()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public static object? GetActivity<T>(this T request)
  {
    switch (request)
    {
      case AuthenticateApiKeyCommand authenticateApiKeyCommand:
        authenticateApiKeyCommand = DeepClone(authenticateApiKeyCommand);
        authenticateApiKeyCommand.Payload.XApiKey = authenticateApiKeyCommand.Payload.XApiKey.Mask();
        return authenticateApiKeyCommand;
      case AuthenticateUserCommand authenticateUserCommand:
        authenticateUserCommand = DeepClone(authenticateUserCommand);
        authenticateUserCommand.Payload.Password = authenticateUserCommand.Payload.Password.Mask();
        return authenticateUserCommand;
      case CreateUserCommand createUserCommand:
        createUserCommand = DeepClone(createUserCommand);
        if (createUserCommand.Payload.Password != null)
        {
          createUserCommand.Payload.Password = createUserCommand.Payload.Password.Mask();
        }
        return createUserCommand;
      case InitializeConfigurationCommand initializeConfiguration:
        initializeConfiguration = DeepClone(initializeConfiguration);
        initializeConfiguration.Payload.User.Password = initializeConfiguration.Payload.User.Password.Mask();
        return initializeConfiguration;
      case ReplaceUserCommand replaceUserCommand:
        replaceUserCommand = DeepClone(replaceUserCommand);
        if (replaceUserCommand.Payload.Password != null)
        {
          replaceUserCommand.Payload.Password = replaceUserCommand.Payload.Password.Mask();
        }
        return replaceUserCommand;
      case ResetUserPasswordCommand resetUserPasswordCommand:
        resetUserPasswordCommand = DeepClone(resetUserPasswordCommand);
        if (resetUserPasswordCommand.Payload.Password != null)
        {
          resetUserPasswordCommand.Payload.Password = resetUserPasswordCommand.Payload.Password.Mask();
        }
        return resetUserPasswordCommand;
      case SignInSessionCommand signInSessionCommand:
        signInSessionCommand = DeepClone(signInSessionCommand);
        signInSessionCommand.Payload.Password = signInSessionCommand.Payload.Password.Mask();
        return signInSessionCommand;
      case UpdateUserCommand updateUserCommand:
        updateUserCommand = DeepClone(updateUserCommand);
        if (updateUserCommand.Payload.Password != null)
        {
          if (updateUserCommand.Payload.Password.Current != null)
          {
            updateUserCommand.Payload.Password.Current = updateUserCommand.Payload.Password.Current.Mask();
          }
          updateUserCommand.Payload.Password.New = updateUserCommand.Payload.Password.New.Mask();
        }
        return updateUserCommand;
    }

    return request;
  }

  private static T DeepClone<T>(T value)
  {
    Type type = value?.GetType() ?? throw new ArgumentNullException(nameof(value));

    string json = JsonSerializer.Serialize(value, type, _serializerOptions);

    return (T?)JsonSerializer.Deserialize(json, type, _serializerOptions)
      ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }
}
