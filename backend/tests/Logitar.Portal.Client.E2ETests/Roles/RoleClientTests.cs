using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Roles;

internal class RoleClientTests : IClientTests
{
  private readonly IRoleClient _client;

  public RoleClientTests(IRoleClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      CreateRolePayload create = new("admin");
      Role role = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      role = await _client.DeleteAsync(role.Id, context.Request)
        ?? throw new InvalidOperationException("The role should not be null.");
      role = await _client.CreateAsync(create, context.Request);
      context.SetRole(role);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      Role? notFound = await _client.ReadAsync(Guid.NewGuid(), uniqueName: null, context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The role should not be found.");
      }
      role = await _client.ReadAsync(role.Id, role.UniqueName, context.Request)
        ?? throw new InvalidOperationException("The role should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchRolesPayload search = new();
      search.Search.Terms.Add(new SearchTerm(role.UniqueName));
      SearchResults<Role> results = await _client.SearchAsync(search, context.Request);
      role = results.Items.Single();
      context.Succeed();

      long version = role.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateRolePayload update = new()
      {
        Description = new Modification<string>("This is the administration role.")
      };
      role = await _client.UpdateAsync(role.Id, update, context.Request)
        ?? throw new InvalidOperationException("The role should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceRolePayload replace = new(role.UniqueName)
      {
        DisplayName = "Administrator",
        Description = null,
        CustomAttributes = role.CustomAttributes
      };
      role = await _client.ReplaceAsync(role.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The role should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
