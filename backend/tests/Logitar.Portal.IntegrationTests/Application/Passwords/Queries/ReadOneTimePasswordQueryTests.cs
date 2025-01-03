﻿using Logitar.Data;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Passwords.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadOneTimePasswordQueryTests : IntegrationTests
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public ReadOneTimePasswordQueryTests() : base()
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

  [Fact(DisplayName = "It should return null when the One-Time Password cannot be found.")]
  public async Task It_should_return_null_when_the_One_Time_Password_cannot_be_found()
  {
    ReadOneTimePasswordQuery query = new(Guid.NewGuid());
    OneTimePasswordModel? oneTimePassword = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(oneTimePassword);
  }

  [Fact(DisplayName = "It should return the One-Time Password when it is found.")]
  public async Task It_should_return_the_One_Time_Password_when_it_is_found()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();

    ReadOneTimePasswordQuery query = new(oneTimePassword.EntityId.ToGuid());
    OneTimePasswordModel? result = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(result);
    Assert.Equal(query.Id, result.Id);
  }

  private async Task<OneTimePassword> CreateOneTimePasswordAsync()
  {
    Password password = _passwordManager.Generate("0123456789", 6, out _);
    OneTimePassword oneTimePassword = new(password);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    return oneTimePassword;
  }
}
