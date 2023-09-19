using Logitar.Portal.Contracts.Http;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Infrastructure.Messages;

[Trait(Traits.Category, Categories.Unit)]
public class SendMessageResultExtensionsTests
{
  private readonly JsonSerializerOptions _serializerOptions = new();

  public SendMessageResultExtensionsTests()
  {
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  [Fact(DisplayName = "It should construct the correct SendMessageResult from an Exception.")]
  public void It_should_construct_the_correct_SendMessageResult_from_an_Exception()
  {
    HttpResponseDetail detail = new()
    {
      Content = @"{""AttemptedValue"":"""",""ErrorCode"":""NotEmptyValidator"",""ErrorMessage"":""The message subject is required."",""PropertyName"":""Subject""}",
      ReasonPhrase = "The message subject is required.",
      StatusCode = 400,
      StatusText = "BadRequest",
      Version = "1.1"
    };
    ArgumentException innerException = new("The message subject is required.", "Subject");
    HttpFailureException exception = new(detail, innerException);

    SendMessageResult result = exception.ToSendMessageResult();

    string serializedInnerException = JsonSerializer.Serialize(innerException.ToSendMessageResult().AsDictionary(), _serializerOptions);
    string serializedData = JsonSerializer.Serialize(new { Detail = JsonSerializer.Serialize(detail, _serializerOptions) }, _serializerOptions);

    Dictionary<string, string> data = result.AsDictionary();
    Assert.Equal(5, data.Count);
    Assert.Equal(serializedData, data["Data"]);
    Assert.Equal(exception.HResult.ToString(), data["HResult"]);
    Assert.Equal(serializedInnerException, data["InnerException"]);
    Assert.Equal(exception.Message, data["Message"]);
    Assert.Equal(exception.GetType().Name, data["Type"]);
  }

  [Fact(DisplayName = "It should construct the correct SendMessageResult from an HttpResponseDetail.")]
  public void It_should_construct_the_correct_SendMessageResult_from_an_HttpResponseDetail()
  {
    HttpResponseDetail detail = new()
    {
      Content = "Hello World!",
      StatusCode = 202,
      StatusText = "Accepted",
      Version = "1.1"
    };

    SendMessageResult result = detail.ToSendMessageResult();

    Dictionary<string, string> data = result.AsDictionary();
    Assert.Equal(4, data.Count);
    Assert.Equal(detail.Content, data["Content"]);
    Assert.Equal(detail.StatusCode.ToString(), data["StatusCode"]);
    Assert.Equal(detail.StatusText, data["StatusText"]);
    Assert.Equal(detail.Version, data["Version"]);
  }
}
