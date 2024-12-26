﻿using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Messages;

public record RecipientModel
{
  public RecipientType Type { get; set; }

  public string? Address { get; set; }
  public string? DisplayName { get; set; }

  public string? PhoneNumber { get; set; }

  public UserModel? User { get; set; }

  public RecipientModel() : this(string.Empty)
  {
  }

  public RecipientModel(string address)
  {
    Address = address;
  }
}
