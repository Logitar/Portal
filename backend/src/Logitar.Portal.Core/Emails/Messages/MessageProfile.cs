using AutoMapper;
using Logitar.Portal.Core.Emails.Messages.Models;

namespace Logitar.Portal.Core.Emails.Messages
{
  internal class MessageProfile : Profile
  {
    public MessageProfile()
    {
      CreateMap<Message, MessageModel>()
        .IncludeBase<Aggregate, AggregateModel>()
        .ForMember(x => x.Result, x => x.MapFrom(y => y.Result == null ? null : y.Result.Select(pair => new ResultDataModel
        {
          Key = pair.Key,
          Value = pair.Value
        })))
        .ForMember(x => x.Variables, x => x.MapFrom(y => y.Variables == null ? null : y.Variables.Select(pair => new VariableModel
        {
          Key = pair.Key,
          Value = pair.Value
        })));
      CreateMap<Recipient, RecipientModel>();
    }
  }
}
