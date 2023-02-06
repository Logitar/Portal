using AutoMapper;
using Logitar.Portal.Application.Accounts.Commands;
using Logitar.Portal.Application.Users.Commands;
using MediatR;
using System.Text.Json;

namespace Logitar.Portal.Application
{
  internal class RequestSerializer : IRequestSerializer
  {
    private const string PasswordMask = "********";

    private readonly IMapper _mapper;

    public RequestSerializer(IMapper mapper)
    {
      _mapper = mapper;
    }

    public string Serialize<T>(IRequest<T> request) => request switch
    {
      ChangePasswordCommand changePasswordCommand => HandleChangePasswordCommand(changePasswordCommand),
      CreateUserCommand createUserCommand => HandleCreateUserCommand(createUserCommand),
      ResetPasswordCommand resetPasswordCommand => HandleResetPasswordCommand(resetPasswordCommand),
      SignInCommand signInCommand => HandleSignInCommand(signInCommand),
      UpdateUserCommand updateUserCommand => HandleUpdateUserCommand(updateUserCommand),
      _ => JsonSerializer.Serialize(request, request.GetType()),
    };

    private string HandleChangePasswordCommand(ChangePasswordCommand source)
    {
      ChangePasswordCommand command = _mapper.Map<ChangePasswordCommand>(source);
      command.Payload.Current = PasswordMask;
      command.Payload.Password = PasswordMask;

      return JsonSerializer.Serialize(command);
    }

    private string HandleCreateUserCommand(CreateUserCommand source)
    {
      CreateUserCommand command = _mapper.Map<CreateUserCommand>(source);
      if (source.Payload.Password != null)
      {
        command.Payload.Password = PasswordMask;
      }

      return JsonSerializer.Serialize(command);
    }

    private string HandleResetPasswordCommand(ResetPasswordCommand source)
    {
      ResetPasswordCommand command = _mapper.Map<ResetPasswordCommand>(source);
      command.Payload.Password = PasswordMask;

      return JsonSerializer.Serialize(command);
    }

    private string HandleSignInCommand(SignInCommand source)
    {
      SignInCommand command = _mapper.Map<SignInCommand>(source);
      command.Payload.Password = PasswordMask;

      return JsonSerializer.Serialize(command);
    }

    private string HandleUpdateUserCommand(UpdateUserCommand source)
    {
      UpdateUserCommand command = _mapper.Map<UpdateUserCommand>(source);
      if (source.Payload.Password != null)
      {
        command.Payload.Password = PasswordMask;
      }

      return JsonSerializer.Serialize(command);
    }
  }
}
