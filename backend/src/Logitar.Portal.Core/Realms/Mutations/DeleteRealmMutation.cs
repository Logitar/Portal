namespace Logitar.Portal.Core.Realms.Mutations
{
  public class DeleteRealmMutation
  {
    public DeleteRealmMutation(Guid id)
    {
      Id = id;
    }

    public Guid Id { get; }
  }
}
