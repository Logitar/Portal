namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RealmEntity : AggregateEntity
  {
    private RealmEntity() : base()
    {
    }

    public int RealmId { get; private set; }

    public string Alias { get; private set; } = string.Empty;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { }
    }
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    public string? DefaultLocale { get; private set; }
    public string? Url { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public bool RequireUniqueEmail { get; private set; }

    public string UsernameSettings { get; private set; } = string.Empty;
    public string PasswordSettings { get; private set; } = string.Empty;

    //public string? PasswordRecoverySenderId { get; private set; } // TODO(fpion): implement when Senders are completed
    //public string? PasswordRecoveryTemplateId { get; private set; } // TODO(fpion): implement when Templates are completed

    public string? GoogleClientId { get; private set; }

    public List<ExternalProviderEntity> ExternalProviders { get; private set; } = new();
    public List<UserEntity> Users { get; private set; } = new();
  }
}
