namespace Logitar.Portal.Contracts.Senders;

public record UpdateSenderInput
{
  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public IEnumerable<Setting>? Settings { get; set; }
}
