﻿using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchSendersQueryTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public SearchSendersQueryTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();

    EmailUnit email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    _sender = new(email, settings);
    _sender.SetDefault();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Senders.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _senderRepository.SaveAsync(_sender);
  }

  [Fact(DisplayName = "It should return empty results when no sender did match.")]
  public async Task It_should_return_empty_results_when_no_sender_did_match()
  {
    SearchSendersPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchSendersQuery query = new(payload);
    SearchResults<Sender> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SenderAggregate notInSearch = new(new EmailUnit(Faker.Internet.Email()), new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()), TenantId)
    {
      DisplayName = new DisplayNameUnit(Faker.Name.FullName())
    };
    notInSearch.Update();
    SenderAggregate notInIds = new(new EmailUnit(Faker.Internet.Email()), new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()), TenantId)
    {
      DisplayName = new DisplayNameUnit(string.Join(' ', Faker.Name.FirstName(), "Sender", Faker.Name.LastName()))
    };
    notInIds.Update();
    SenderAggregate sender1 = new(new EmailUnit(Faker.Internet.Email()), new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()), TenantId)
    {
      DisplayName = new DisplayNameUnit(string.Join(' ', Faker.Name.FirstName(), "Sender", Faker.Name.LastName()))
    };
    sender1.Update();
    SenderAggregate sender2 = new(new EmailUnit(Faker.Internet.Email()), new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()), TenantId)
    {
      DisplayName = new DisplayNameUnit(string.Join(' ', Faker.Name.FirstName(), "Sender", Faker.Name.LastName()))
    };
    sender2.Update();
    await _senderRepository.SaveAsync([notInSearch, notInIds, sender1, sender2]);

    SetRealm();

    SearchSendersPayload payload = new()
    {
      Provider = SenderProvider.SendGrid,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> senderIds = (await _senderRepository.LoadAsync()).Select(sender => sender.Id.ToGuid());
    payload.Ids.AddRange(senderIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("%sender%"));
    payload.Sort.Add(new SenderSortOption(SenderSort.EmailAddress, isDescending: false));
    SearchSendersQuery query = new(payload);
    SearchResults<Sender> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    Sender sender = Assert.Single(results.Items);
    SenderAggregate expected = new[] { sender1, sender2 }.OrderBy(s => s.Email?.Address).Skip(1).Single();
    Assert.Equal(expected.Id.ToGuid(), sender.Id);
  }
}
