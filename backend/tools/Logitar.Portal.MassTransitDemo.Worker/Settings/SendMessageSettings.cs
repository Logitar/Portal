using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.MassTransitDemo.Worker.Settings;

internal record SendMessageSettings
{
  public string? Realm { get; set; }
  public Guid? SenderId { get; set; }
  public string Template { get; set; }

  public List<RecipientPayload> Recipients { get; set; }

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public List<Variable> Variables { get; set; }

  public SendMessageSettings() : this(string.Empty)
  {
  }

  public SendMessageSettings(string template)
  {
    Template = template;
    Recipients = [];
    Variables = [];
  }
}
