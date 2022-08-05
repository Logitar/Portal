using AutoMapper;
using Logitar.Portal.Core.Realms.Models;

namespace Logitar.Portal.Core.Realms
{
  internal class RealmProfile : Profile
  {
    public RealmProfile()
    {
      CreateMap<Realm, RealmModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.PasswordRecoverySenderId, x => x.MapFrom(y => y.PasswordRecoverySender == null ? (Guid?)null : y.PasswordRecoverySender.Id))
        .ForMember(x => x.PasswordRecoveryTemplateId, x => x.MapFrom(y => y.PasswordRecoveryTemplate == null ? (Guid?)null : y.PasswordRecoveryTemplate.Id));
      CreateMap<RealmModel, RealmSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
    }
  }
}
