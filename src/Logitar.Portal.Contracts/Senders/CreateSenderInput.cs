namespace Logitar.Portal.Contracts.Senders;

public record CreateSenderInput
{
  public string? Realm { get; set; }

  public ProviderType Provider { get; set; }

  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public IEnumerable<Setting>? Settings { get; set; }
}
