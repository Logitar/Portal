﻿using System.Text;

namespace Logitar.Portal.Core;

public class InvalidTimeZoneEntryException : Exception, IPropertyFailure
{
  public InvalidTimeZoneEntryException(string value, string paramName, Exception innerException)
    : base(GetMessage(value, paramName), innerException)
  {
    Data[nameof(Value)] = value;
    Data[nameof(ParamName)] = paramName;
  }

  public string ParamName => (string)Data[nameof(ParamName)]!;
  public string Value => (string)Data[nameof(Value)]!;

  private static string GetMessage(string value, string paramName)
  {
    StringBuilder message = new();

    message.Append("The time zone entry '").Append(value).AppendLine("' is not valid.");
    message.Append("ParamName: ").Append(paramName).AppendLine();

    return message.ToString();
  }
}