using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Templates;

public class TemplateNotInTenantException : Exception
{
  private const string ErrorMessage = "The specified template is not in the specified tenant.";

  public Guid TemplateId
  {
    get => (Guid)Data[nameof(TemplateId)]!;
    private set => Data[nameof(TemplateId)] = value;
  }
  public Guid? ExpectedTenantId
  {
    get => (Guid?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }
  public Guid? ActualTenantId
  {
    get => (Guid?)Data[nameof(ActualTenantId)];
    private set => Data[nameof(ActualTenantId)] = value;
  }

  public TemplateNotInTenantException(Template template, TenantId? expectedTenant) : base(BuildMessage(template, expectedTenant))
  {
    TemplateId = template.EntityId.ToGuid();
    ExpectedTenantId = expectedTenant?.ToGuid();
    ActualTenantId = template.TenantId?.ToGuid();
  }

  private static string BuildMessage(Template template, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TemplateId), template.EntityId.ToGuid())
    .AddData(nameof(ExpectedTenantId), expectedTenant?.ToGuid(), "<null>")
    .AddData(nameof(ActualTenantId), template.TenantId?.ToGuid(), "<null>")
    .Build();
}
