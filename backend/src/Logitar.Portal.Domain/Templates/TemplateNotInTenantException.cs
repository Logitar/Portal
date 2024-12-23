using Logitar.Identity.Core;

namespace Logitar.Portal.Domain.Templates;

public class TemplateNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified template is not in the specified tenant.";

  public string TemplateId
  {
    get => (string)Data[nameof(TemplateId)]!;
    private set => Data[nameof(TemplateId)] = value;
  }
  public string? ExpectedTenantId
  {
    get => (string?)Data[nameof(ExpectedTenantId)];
    private set => Data[nameof(ExpectedTenantId)] = value;
  }
  public string? ActualTenantId
  {
    get => (string?)Data[nameof(ActualTenantId)];
    private set => Data[nameof(ActualTenantId)] = value;
  }

  public TemplateNotInTenantException(TemplateAggregate template, TenantId? expectedTenant) : base(BuildMessage(template, expectedTenant))
  {
    TemplateId = template.Id.Value;
    ExpectedTenantId = expectedTenant?.Value;
    ActualTenantId = template.TenantId?.Value;
  }

  private static string BuildMessage(TemplateAggregate template, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TemplateId), template.Id)
    .AddData(nameof(ExpectedTenantId), expectedTenant, "<null>")
    .AddData(nameof(ActualTenantId), template.TenantId, "<null>")
    .Build();
}
