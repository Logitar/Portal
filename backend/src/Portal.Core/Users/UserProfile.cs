using AutoMapper;
using Portal.Core.Users.Models;

namespace Portal.Core.Users
{
  internal class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<User, UserModel>()
        .IncludeBase<Aggregate, AggregateModel>();
    }
  }
}
