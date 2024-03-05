﻿using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ReplaceUserCommand(Guid Id, ReplaceUserPayload Payload, long? Version) : ApplicationRequest, IRequest<User?>
{
  public override IActivity GetActivity()
  {
    if (Payload.Password == null)
    {
      return base.GetActivity();
    }

    ReplaceUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}
