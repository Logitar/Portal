using AutoMapper;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions
{
  internal class SessionProfile : Profile
  {
    public SessionProfile()
    {
      CreateMap<Session, SessionModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<SessionModel, SessionSummary>()
        .IncludeBase<AggregateModel, AggregateSummary>();
      CreateMap<UserModel, SessionUserSummary>();
    }
  }
}
