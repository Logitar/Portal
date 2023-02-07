using Logitar.Portal.Application.Accounts.Commands;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Users.Commands;
using MediatR;
using System.Text.Json;

namespace Logitar.Portal.Application
{
  internal class RequestSerializer : IRequestSerializer
  {
    private const string PasswordMask = "********";

    public string Serialize<T>(IRequest<T> request) => request switch
    {
      ChangePasswordCommand changePasswordCommand => MaskChangePasswordCommand(changePasswordCommand),
      CreateUserCommand createUserCommand => MaskCreateUserCommand(createUserCommand),
      InitializeConfigurationCommand initializeConfigurationCommand => MaskInitializeConfigurationCommand(initializeConfigurationCommand),
      ResetPasswordCommand resetPasswordCommand => MaskResetPasswordCommand(resetPasswordCommand),
      SignInCommand signInCommand => MaskSignInCommand(signInCommand),
      UpdateUserCommand updateUserCommand => MaskUpdateUserCommand(updateUserCommand),
      _ => SerializeToJson(request),
    };

    private static string MaskChangePasswordCommand(ChangePasswordCommand source)
    {
      ChangePasswordCommand command = Clone(source);
      command.Payload.Current = PasswordMask;
      command.Payload.Password = PasswordMask;

      return SerializeToJson(command);
    }

    private static string MaskCreateUserCommand(CreateUserCommand source)
    {
      CreateUserCommand command = Clone(source);
      if (source.Payload.Password != null)
      {
        command.Payload.Password = PasswordMask;
      }

      return SerializeToJson(command);
    }

    private static string MaskInitializeConfigurationCommand(InitializeConfigurationCommand source)
    {
      InitializeConfigurationCommand command = Clone(source);
      command.Payload.User.Password = PasswordMask;

      return SerializeToJson(command);
    }

    private static string MaskResetPasswordCommand(ResetPasswordCommand source)
    {
      ResetPasswordCommand command = Clone(source);
      command.Payload.Password = PasswordMask;

      return SerializeToJson(command);
    }

    private static string MaskSignInCommand(SignInCommand source)
    {
      SignInCommand command = Clone(source);
      command.Payload.Password = PasswordMask;

      return SerializeToJson(command);
    }

    private static string MaskUpdateUserCommand(UpdateUserCommand source)
    {
      UpdateUserCommand command = Clone(source);
      if (source.Payload.Password != null)
      {
        command.Payload.Password = PasswordMask;
      }

      return SerializeToJson(command);
    }

    private static T Clone<T>(T source) where T : notnull => JsonSerializer.Deserialize<T>(SerializeToJson(source))
      ?? throw new InvalidOperationException($"The object '{source}' could not be cloned.");

    private static string SerializeToJson(object obj) => JsonSerializer.Serialize(obj, obj.GetType());
  }
}
