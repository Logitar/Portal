using FluentValidation.Results;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Domain.Templates;

public class TemplateNotInRealmException : Exception
{
  private const string ErrorMessage = "The specified template must be in the specified realm.";

  public TemplateNotInRealmException(TemplateAggregate template, RealmAggregate? realm, string propertyName)
    : base(BuildMessage(template, realm, propertyName))
  {
    Template = template.ToString();
    Realm = realm?.ToString();
    PropertyName = propertyName;
  }

  public string Template
  {
    get => (string)Data[nameof(Template)]!;
    private set => Data[nameof(Template)] = value;
  }
  public string? Realm
  {
    get => (string?)Data[nameof(Realm)];
    private set => Data[nameof(Realm)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Template)
  {
    ErrorCode = "TemplateNotInRealm",
    CustomState = new { Realm }
  };

  private static string BuildMessage(TemplateAggregate template, RealmAggregate? realm, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("Template: ").Append(template).AppendLine();
    message.Append("Realm: ").AppendLine(realm?.ToString() ?? "<null>");
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
