namespace Portal.Infrastructure.Users
{
  internal class Error
  {
    public Error(string code, string? description = null)
    {
      Code = code ?? throw new ArgumentNullException(nameof(code));
      Description = description;
    }

    public string Code { get; }
    public string? Description { get; }

    public override string ToString() => Description == null ? Code : $"{Code}: {Description}";
  }
}
