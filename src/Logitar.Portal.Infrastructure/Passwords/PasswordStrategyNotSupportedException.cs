namespace Logitar.Portal.Infrastructure.Passwords;

public class PasswordStrategyNotSupportedException : Exception
{
  public PasswordStrategyNotSupportedException(string key)
    : base($"The password strategy '{key}' is not supported.")
  {
    Key = key;
  }

  public string Key
  {
    get => (string)Data[nameof(Key)]!;
    private set => Data[nameof(Key)] = value;
  }
}
