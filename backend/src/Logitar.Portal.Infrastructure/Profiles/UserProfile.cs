using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<UserEntity, UserModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.DisabledBy, x => x.Ignore()) // TODO(fpion): implement Actors
        .ForMember(x => x.EmailConfirmedBy, x => x.Ignore()) // TODO(fpion): implement Actors
        .ForMember(x => x.PhoneNumberConfirmedBy, x => x.Ignore()); // TODO(fpion): implement Actors
    }
  }
}
