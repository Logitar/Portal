using AutoMapper;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Core.Users
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<ExternalProvider, ExternalProviderModel>();
      CreateMap<User, UserModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<UserModel, UserSummary>()
        .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt ?? y.CreatedAt));

      CreateMap<PasswordSettings, PasswordSettingsModel>();

      CreateMap<CreateUserPayload, CreateUserSecurePayload>();
      CreateMap<UpdateUserPayload, UpdateUserSecurePayload>();
    }
  }
}
