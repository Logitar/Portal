namespace Logitar.Portal.Core.Users.Models
{
  public class ExternalProviderModel
  {
    public Guid Id { get; set; }

    public DateTime AddedAt { get; set; }
    public Guid AddedById { get; set; }

    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;

    public string? DisplayName { get; set; }
  }
}
