using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Templates;

public class TemplateNotFoundException : Exception
{
  private const string ErrorMessage = "The specified template could not be found.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string Identifier
  {
    get => (string)Data[nameof(Identifier)]!;
    private set => Data[nameof(Identifier)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public TemplateNotFoundException(TenantId? tenantId, string identifier, string? propertyName = null) : base(BuildMessage(tenantId, identifier, propertyName))
  {
    TenantId = tenantId?.ToGuid();
    Identifier = identifier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(TenantId? tenantId, string identifier, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId?.ToGuid(), "<null>")
    .AddData(nameof(Identifier), identifier)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
