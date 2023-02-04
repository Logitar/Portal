using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Infrastructure.Entities;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class DictionaryProfile : Profile
  {
    public DictionaryProfile()
    {
      CreateMap<DictionaryEntity, DictionaryModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.Entries, x => x.MapFrom(GetEntries));
    }

    private static IEnumerable<EntryModel> GetEntries(DictionaryEntity dictionary, DictionaryModel model)
    {
      if (dictionary.Entries == null)
      {
        return Enumerable.Empty<EntryModel>();
      }

      Dictionary<string, string> entries = JsonSerializer.Deserialize<Dictionary<string, string>>(dictionary.Entries)
        ?? throw new InvalidOperationException($"The dictionary 'Id={dictionary.DictionaryId}' entries could not be deserialized.");

      return entries.Select(entry => new EntryModel
      {
        Key = entry.Key,
        Value = entry.Value
      });
    }
  }
}
