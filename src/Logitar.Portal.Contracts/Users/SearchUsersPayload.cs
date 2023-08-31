namespace Logitar.Portal.Contracts.Users;

public record SearchUsersPayload : SearchPayload
{
  public string? Realm { get; set; }

  public bool? HasPassword { get; set; }
  public bool? IsConfirmed { get; set; }
  public bool? IsDisabled { get; set; }

  public new IEnumerable<UserSortOption> Sort { get; set; } = Enumerable.Empty<UserSortOption>();
}
