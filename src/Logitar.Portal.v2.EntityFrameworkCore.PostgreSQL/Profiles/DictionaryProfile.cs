using AutoMapper;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal class DictionaryProfile : Profile
{
  public DictionaryProfile()
  {
    CreateMap<DictionaryEntity, Dictionary>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.Entries, x => x.MapFrom(GetEntries));
  }

  private static IEnumerable<Entry> GetEntries(DictionaryEntity dictionary, Dictionary _)
  {
    if (dictionary.Entries == null)
    {
      return Enumerable.Empty<Entry>();
    }

    Dictionary<string, string> entries = JsonSerializer.Deserialize<Dictionary<string, string>>(dictionary.Entries)
      ?? throw new InvalidOperationException($"The entries deserialization failed: '{dictionary.Entries}'.");

    return entries.Select(pair => new Entry
    {
      Key = pair.Key,
      Value = pair.Value
    });
  }
}
