namespace Logitar.Portal.Application.Realms.Mutations
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
