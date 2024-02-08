using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should delete an existing One-Time Password.")]
  public async Task It_should_delete_an_existing_One_Time_Password()
  {
    OneTimePasswordAggregate oneTimePassword = await CreateOneTimePasswordAsync();

    DeleteOneTimePasswordCommand command = new(oneTimePassword.Id.AggregateId.ToGuid());
    OneTimePassword? deleted = await Mediator.Send(command);
    Assert.NotNull(deleted);
    Assert.Equal(command.Id, deleted.Id);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password cannot be found.")]
  public async Task It_should_return_null_when_the_One_Time_Password_cannot_be_found()
  {
    DeleteOneTimePasswordCommand command = new(Guid.NewGuid());
    OneTimePassword? oneTimePassword = await Mediator.Send(command);
    Assert.Null(oneTimePassword);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password is in another tenant.")]
  public async Task It_should_return_null_when_the_One_Time_Password_is_in_another_tenant()
  {
    OneTimePasswordAggregate oneTimePassword = await CreateOneTimePasswordAsync();

    SetRealm();

    DeleteOneTimePasswordCommand command = new(oneTimePassword.Id.AggregateId.ToGuid());
    OneTimePassword? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  private async Task<OneTimePasswordAggregate> CreateOneTimePasswordAsync()
  {
    Password password = _passwordManager.Generate("0123456789", 6, out _);
    OneTimePasswordAggregate oneTimePassword = new(password);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    return oneTimePassword;
  }
}
