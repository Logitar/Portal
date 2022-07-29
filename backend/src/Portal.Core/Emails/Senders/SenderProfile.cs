using AutoMapper;
using Portal.Core.Emails.Senders.Models;

namespace Portal.Core.Emails.Senders
{
  internal class SenderProfile : Profile
  {
    public SenderProfile()
    {
      CreateMap<Sender, SenderModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.Settings, x => x.MapFrom(y => y.Settings.Select(pair => new SettingModel
        {
          Key = pair.Key,
          Value = pair.Value
        })));
    }
  }
}
