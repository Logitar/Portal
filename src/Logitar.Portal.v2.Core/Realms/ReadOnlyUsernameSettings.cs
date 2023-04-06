namespace Logitar.Portal.v2.Core.Realms;

internal record ReadOnlyUsernameSettings(
  string? AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+");
