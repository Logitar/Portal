﻿namespace Logitar.Portal.Client;

public record BasicCredentials
{
  public string Username { get; set; }
  public string Password { get; set; }

  public BasicCredentials() : this(string.Empty, string.Empty)
  {
  }

  public BasicCredentials(string username, string password)
  {
    Username = username;
    Password = password;
  }
}
