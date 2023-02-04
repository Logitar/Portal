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
        .ForMember(x => x.DisabledBy, x => x.MapFrom(y => Actor.GetActorModel(y.DisabledById, y.DisabledBy)))
        .ForMember(x => x.EmailConfirmedBy, x => x.MapFrom(y => Actor.GetActorModel(y.EmailConfirmedById, y.EmailConfirmedBy)))
        .ForMember(x => x.PhoneNumberConfirmedBy, x => x.MapFrom(y => Actor.GetActorModel(y.PhoneNumberConfirmedById, y.PhoneNumberConfirmedBy)));
      CreateMap<ExternalProviderEntity, ExternalProviderModel>()
        .ForMember(x => x.AddedBy, x => x.MapFrom(y => Actor.GetActorModel(y.AddedById, y.AddedBy)));
    }
  }
}
