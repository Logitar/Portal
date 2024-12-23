using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Actors;

public class ActorModel
{
  public static readonly ActorModel System = new("System");

  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public ActorModel() : this(string.Empty)
  {
  }

  public ActorModel(string displayName)
  {
    DisplayName = displayName;
  }

  public ActorModel(ApiKeyModel apiKey) : this(apiKey.DisplayName)
  {
    Id = apiKey.Id;
    Type = ActorType.ApiKey;
  }

  public ActorModel(UserModel user) : this(user.FullName ?? user.UniqueName)
  {
    Id = user.Id;
    Type = ActorType.User;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }

  public override bool Equals(object? obj) => obj is ActorModel actor && actor.Id == Id;
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
