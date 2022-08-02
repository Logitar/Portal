using AutoMapper;
using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Web.Models.Api.Realm
{
  internal class RealmProfile : Profile
  {
    public RealmProfile()
    {
      CreateMap<RealmModel, RealmSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
