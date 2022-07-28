using AutoMapper;
using Portal.Core.Realms.Models;

namespace Portal.Core.Realms
{
  internal class RealmProfile : Profile
  {
    public RealmProfile()
    {
      CreateMap<Realm, RealmModel>()
        .IncludeBase<Aggregate, AggregateModel>();
    }
  }
}
