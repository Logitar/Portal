﻿using FluentValidation;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages.Validators;

namespace Logitar.Portal.Domain.Messages;

public record RecipientUnit
{
  public RecipientType Type { get; }

  public string? Address { get; }
  public string? DisplayName { get; }

  public string? PhoneNumber { get; }

  public UserId? UserId { get; }
  [JsonIgnore]
  public UserAggregate? User { get; }

  [JsonConstructor]
  public RecipientUnit(RecipientType type = RecipientType.To, string? address = null, string? displayName = null, string? phoneNumber = null, UserId? userId = null)
  {
    Type = type;
    Address = address?.CleanTrim();
    DisplayName = displayName?.CleanTrim();
    PhoneNumber = phoneNumber?.CleanTrim();
    UserId = userId;
    new RecipientValidator().ValidateAndThrow(this);
  }

  public RecipientUnit(UserAggregate user, RecipientType type = RecipientType.To)
    : this(type, user.Email?.Address, user.FullName, user.Phone?.FormatToE164(), user.Id)
  {
    User = user;
  }
}
