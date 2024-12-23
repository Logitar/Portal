﻿using FluentValidation;
using FluentValidation.Validators;

namespace Logitar.Portal.Domain.Validators;

internal class SlugValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "SlugValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    return string.IsNullOrEmpty(value) || value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
  }
}
