using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Dictionaries;

public class DictionaryAlreadyExistsException : Exception
{
  public const string ErrorMessage = "The specified dictionary already exists.";

  public Guid? RealmId
  {
    get => (Guid?)Data[nameof(RealmId)];
    private set => Data[nameof(RealmId)] = value;
  }
  public string Locale
  {
    get => (string)Data[nameof(Locale)]!;
    private set => Data[nameof(Locale)] = value;
  }

  public DictionaryAlreadyExistsException(TenantId? tenantId, Locale locale) : base(BuildMessage(tenantId, locale))
  {
    RealmId = tenantId?.ToGuid();
    Locale = locale.Code;
  }

  private static string BuildMessage(TenantId? tenantId, Locale locale) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), tenantId?.ToGuid(), "<null>")
    .AddData(nameof(Locale), locale)
    .Build();
}
