namespace Logitar.Portal.Contracts;

public record UniqueNameSettings
{
  public string? AllowedCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}
