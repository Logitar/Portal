namespace Logitar.Portal.Contracts.Users;

public record RecoverPasswordResult
{
  public Guid MessageId { get; set; }
  public User User { get; set; } = new();
}
