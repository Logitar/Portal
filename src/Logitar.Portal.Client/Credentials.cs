using System.Text;

namespace Logitar.Portal.Client;

public record Credentials
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  public string Encode() => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
}
