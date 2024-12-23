using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Application.Dictionaries;

public class DictionaryAlreadyExistsException : Exception
{
  public const string ErrorMessage = "The specified dictionary already exists.";

  public TenantId? TenantId
  {
    get => TenantId.TryCreate((string)Data[nameof(TenantId)]!);
    private set => Data[nameof(TenantId)] = value?.Value;
  }
  public Locale Locale
  {
    get => new((string)Data[nameof(Locale)]!);
    private set => Data[nameof(Locale)] = value.Code;
  }

  public DictionaryAlreadyExistsException(TenantId? tenantId, Locale locale) : base(BuildMessage(tenantId, locale))
  {
    TenantId = tenantId;
    Locale = locale;
  }

  private static string BuildMessage(TenantId? tenantId, Locale locale) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.Value, "<null>")
    .AddData(nameof(Locale), locale.Code)
    .Build();
}
