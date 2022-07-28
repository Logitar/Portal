namespace Portal.Core.Settings
{
  internal class UserSettings
  {
    public string AllowedUserNameCharacters { get; set; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    public bool RequireUniqueEmail { get; set; } = false;
  }
}
