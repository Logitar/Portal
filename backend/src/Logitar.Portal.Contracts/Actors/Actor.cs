namespace Logitar.Portal.Contracts.Actors;

public class Actor
{
  public string Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public Actor() : this("SYSTEM", "System")
  {
  }

  public Actor(string id, string displayName)
  {
    Id = id;
    DisplayName = displayName;
  }

  public override bool Equals(object? obj) => obj is Actor actor && actor.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString()
  {
    StringBuilder s = new();
    s.Append(DisplayName);
    if (EmailAddress != null)
    {
      s.Append(" <").Append(EmailAddress).Append('>');
    }
    s.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return s.ToString();
  }
}
