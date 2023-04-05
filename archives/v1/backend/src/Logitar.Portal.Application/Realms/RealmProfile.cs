using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms
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
        .IncludeBase<AggregateModel, AggregateSummary>();

      CreateMap<PasswordSettings, PasswordSettingsModel>();
    }
  }
}
