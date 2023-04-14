using Logitar.Portal.Core.Configurations.Commands;
using Logitar.Portal.Core.Logging;
using Logitar.Portal.Core.Sessions.Commands;
using Logitar.Portal.Core.Users.Commands;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Core;

internal static class RequestExtensions
{
  private static readonly JsonSerializerOptions _options = new();
  static RequestExtensions() => _options.Converters.Add(new JsonStringEnumConverter());

  public static Activity GetActivity<T>(this IRequest<T> request)
  {
    switch (request)
    {
      case ChangePassword changePassword:
        changePassword = DeepClone(changePassword)!;
        changePassword.Input.Current = changePassword.Input.Current.Mask();
        changePassword.Input.Password = changePassword.Input.Password.Mask();
        return CreateActivity(changePassword);
      case CreateUser createUser:
        createUser = DeepClone(createUser);
        createUser.Input.Password = createUser.Input.Password?.Mask();
        return CreateActivity(createUser);
      case InitializeConfiguration initializeConfiguration:
        initializeConfiguration = DeepClone(initializeConfiguration);
        initializeConfiguration.Input.User.Password = initializeConfiguration.Input.User.Password.Mask();
        return CreateActivity(initializeConfiguration);
      case ResetPassword resetPassword:
        resetPassword = DeepClone(resetPassword);
        resetPassword.Input.Password = resetPassword.Input.Password.Mask();
        return CreateActivity(resetPassword);
      case SignIn signIn:
        signIn = DeepClone(signIn);
        signIn.Input.Password = signIn.Input.Password.Mask();
        return CreateActivity(signIn);
      case UpdateUser updateUser:
        updateUser = DeepClone(updateUser);
        updateUser.Input.Password = updateUser.Input.Password?.Mask();
        return CreateActivity(updateUser);
      default:
        return CreateActivity(request);
    }
  }

  private static Activity CreateActivity<T>(IRequest<T> request)
  {
    Type type = request.GetType();

    return new(type, JsonSerializer.Serialize(request, type, _options));
  }
  private static T DeepClone<T>(T value)
  {
    Type type = value?.GetType() ?? throw new ArgumentNullException(nameof(value));

    string json = JsonSerializer.Serialize(value, type, _options);

    return (T?)JsonSerializer.Deserialize(json, type, _options)
      ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }
}
