﻿using Bogus;

namespace Logitar.Portal;

public static class PersonExtensions
{
  private static readonly Random _random = new();

  public static string BuildHealthInsuranceNumber(this Person person)
  {
    StringBuilder hin = new();

    hin.Append(person.LastName[..3].ToUpperInvariant());
    hin.Append(person.FirstName[..1].ToUpperInvariant());
    hin.Append(person.DateOfBirth.Year % 100);
    hin.Append(person.DateOfBirth.Month.ToString("00"));
    hin.Append(person.DateOfBirth.Day.ToString("00"));
    hin.Append(_random.Next(0, 99 + 1).ToString("00"));

    return hin.ToString();
  }
}
