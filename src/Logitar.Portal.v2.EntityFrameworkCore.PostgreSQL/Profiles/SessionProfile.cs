using AutoMapper;
using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal class SessionProfile : Profile
{
  public SessionProfile()
  {
    CreateMap<SessionEntity, Session>()
      .IncludeBase<AggregateEntity, Aggregate>()
      .ForMember(x => x.Id, x => x.MapFrom(MappingHelper.GetId))
      .ForMember(x => x.SignedOutBy, x => x.MapFrom(y => MappingHelper.GetActor(y.SignedOutById, y.SignedOutBy)))
      .ForMember(x => x.CustomAttributes, x => x.MapFrom(MappingHelper.GetCustomAttributes));
  }
}
