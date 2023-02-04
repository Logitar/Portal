using AutoMapper;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Infrastructure.Entities;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Profiles
{
  internal class MessageProfile : Profile
  {
    public MessageProfile()
    {
      CreateMap<MessageEntity, MessageModel>()
        .IncludeBase<AggregateEntity, AggregateModel>()
        .ForMember(x => x.Errors, x => x.MapFrom(GetErrors))
        .ForMember(x => x.Result, x => x.MapFrom(GetResult))
        .ForMember(x => x.Variables, x => x.MapFrom(GetVariables));
      CreateMap<RecipientEntity, RecipientModel>();
    }

    private static IEnumerable<ErrorModel> GetErrors(MessageEntity message, MessageModel model)
    {
      if (message.Errors == null)
      {
        return Enumerable.Empty<ErrorModel>();
      }

      IEnumerable<Error> errors = JsonSerializer.Deserialize<IEnumerable<Error>>(message.Errors)
        ?? throw new InvalidOperationException($"The message 'Id={message.MessageId}' errors could not be deserialized.");

      return errors.Select(e => new ErrorModel
      {
        Code = e.Code,
        Data = e.Data?.Select(d => new ErrorDataModel
        {
          Key = d.Key,
          Value = d.Value
        }) ?? Enumerable.Empty<ErrorDataModel>(),
        Description = e.Description
      });
    }

    private static IEnumerable<ResultDataModel> GetResult(MessageEntity message, MessageModel model)
    {
      if (message.Result == null)
      {
        return Enumerable.Empty<ResultDataModel>();
      }

      SendMessageResult result = JsonSerializer.Deserialize<SendMessageResult>(message.Result)
        ?? throw new InvalidOperationException();

      return result.Select(r => new ResultDataModel
      {
        Key = r.Key,
        Value = r.Value
      });
    }

    private static IEnumerable<VariableModel> GetVariables(MessageEntity message, MessageModel model)
    {
      if (message.Variables == null)
      {
        return Enumerable.Empty<VariableModel>();
      }

      Dictionary<string, string?> variables = JsonSerializer.Deserialize<Dictionary<string, string?>>(message.Variables)
        ?? throw new InvalidOperationException($"The message 'Id={message.MessageId}' variables could not be deserialized.");

      return variables.Select(v => new VariableModel
      {
        Key = v.Key,
        Value = v.Value
      });
    }
  }
}
