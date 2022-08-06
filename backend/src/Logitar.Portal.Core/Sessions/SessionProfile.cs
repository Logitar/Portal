using AutoMapper;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Sessions
{
  internal class SessionProfile : Profile
  {
    public SessionProfile()
    {
      CreateMap<Session, SessionModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<SessionModel, SessionSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));
      CreateMap<UserModel, SessionUserSummary>();
    }
  }
}
