using AutoMapper;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;

namespace Portal.Core.Users
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<User, UserModel>()
        .IncludeBase<Aggregate, AggregateModel>();
      CreateMap<PasswordSettings, PasswordSettingsModel>();
      CreateMap<CreateUserPayload, CreateUserSecurePayload>();
      CreateMap<UpdateUserPayload, UpdateUserSecurePayload>();
    }
  }
}
