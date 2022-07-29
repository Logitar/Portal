using Portal.Core.Users;

namespace Portal.Infrastructure.Emails.Messages
{
  public class TemplateModel
  {
    private readonly IReadOnlyDictionary<string, string?> _variables;

    public TemplateModel(User? user = null, IReadOnlyDictionary<string, string?>? variables = null)
    {
      User = user;
      _variables = variables ?? new Dictionary<string, string?>();
    }

    public User? User { get; }

    public string? Variable(string key) => _variables.TryGetValue(key, out string? value) ? value : null;
  }
}
