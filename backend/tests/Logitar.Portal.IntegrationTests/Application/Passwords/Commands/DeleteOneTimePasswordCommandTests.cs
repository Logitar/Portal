using Logitar.Data;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Application.OneTimePasswords.Commands;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Passwords.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteOneTimePasswordCommandTests : IntegrationTests
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public DeleteOneTimePasswordCommandTests() : base()
  {
    _oneTimePasswordRepository = ServiceProvider.GetRequiredService<IOneTimePasswordRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.OneTimePasswords.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing One-Time Password.")]
  public async Task It_should_delete_an_existing_One_Time_Password()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();

    DeleteOneTimePasswordCommand command = new(oneTimePassword.EntityId.ToGuid());
    OneTimePasswordModel? deleted = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(deleted);
    Assert.Equal(command.Id, deleted.Id);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password cannot be found.")]
  public async Task It_should_return_null_when_the_One_Time_Password_cannot_be_found()
  {
    DeleteOneTimePasswordCommand command = new(Guid.NewGuid());
    OneTimePasswordModel? oneTimePassword = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(oneTimePassword);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password is in another tenant.")]
  public async Task It_should_return_null_when_the_One_Time_Password_is_in_another_tenant()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();

    SetRealm();

    DeleteOneTimePasswordCommand command = new(oneTimePassword.EntityId.ToGuid());
    OneTimePasswordModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  private async Task<OneTimePassword> CreateOneTimePasswordAsync()
  {
    Password password = _passwordManager.Generate("0123456789", 6, out _);
    OneTimePassword oneTimePassword = new(password);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    return oneTimePassword;
  }
}
