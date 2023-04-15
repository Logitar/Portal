using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Dictionaries.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries
{
  internal class DictionaryProfile : Profile
  {
    public DictionaryProfile()
    {
      CreateMap<Dictionary, DictionaryModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.Entries, x => x.MapFrom(y => y.Entries.Select(z => new EntryModel
        {
          Key = z.Key,
          Value = z.Value
        })));
      CreateMap<DictionaryModel, DictionarySummary>()
        .IncludeBase<AggregateModel, AggregateSummary>()
        .ForMember(x => x.Entries, x => x.MapFrom(y => y.Entries.Count()))
        .ForMember(x => x.Realm, x => x.MapFrom(y => y.Realm == null ? null : y.Realm.Name));
    }
  }
}
