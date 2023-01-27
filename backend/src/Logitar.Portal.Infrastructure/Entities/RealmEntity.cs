namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RealmEntity : AggregateEntity
  {
    public string Alias { get; private set; } = null!;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { }
    }
    public string DisplayName { get; private set; } = null!;
    public string? Description { get; private set; }

    public string? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    // TODO(fpion): UsernameSettings
    // TODO(fpion): PasswordSettings

    // TODO(fpion): PasswordRecoverySender
    // TODO(fpion): PasswordRecoveryTemplate

    public string? GoogleClientId { get; private set; }
  }
}
