﻿using System.Text;

namespace Logitar.Portal.Application.Users
{
  public class EmailAlreadyUsedException : Exception
  {
    public EmailAlreadyUsedException(string email, string paramName)
      : base(GetMessage(email, paramName))
    {
      Data["Email"] = email;
      Data["paramName"] = paramName;
    }

    private static string GetMessage(string email, string paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified email is already used.");
      message.AppendLine($"Email: {email}");
      message.AppendLine($"ParamName: {paramName}");

      return message.ToString();
    }
  }
}
