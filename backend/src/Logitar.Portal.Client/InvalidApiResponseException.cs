using Logitar.Portal.Contracts;

namespace Logitar.Portal.Client;

public class InvalidApiResponseException : Exception
{
  private const string ErrorMessage = "An API response was expected, but it was null after deserialization.";

  public string ClientType
  {
    get => (string)Data[nameof(ClientType)]!;
    private set => Data[nameof(ClientType)] = value;
  }
  public string MethodName
  {
    get => (string)Data[nameof(MethodName)]!;
    private set => Data[nameof(MethodName)] = value;
  }
  public string HttpMethod
  {
    get => (string)Data[nameof(HttpMethod)]!;
    private set => Data[nameof(HttpMethod)] = value;
  }
  public string Uri
  {
    get => (string)Data[nameof(Uri)]!;
    private set => Data[nameof(Uri)] = value;
  }
  public string? Content
  {
    get => (string?)Data[nameof(Content)];
    private set => Data[nameof(Content)] = value;
  }
  public string? User
  {
    get => (string?)Data[nameof(User)];
    private set => Data[nameof(User)] = value;
  }

  public InvalidApiResponseException(Type clientType, string methodName, HttpMethod httpMethod, Uri uri, string? content, IRequestContext? context)
    : base(BuildMessage(clientType, methodName, httpMethod, uri, content, context))
  {
    ClientType = clientType.GetNamespaceQualifiedName();
    MethodName = methodName;
    HttpMethod = httpMethod.ToString();
    Uri = uri.ToString();
    Content = content;
    SetContext(context);
  }
  private void SetContext(IRequestContext? context)
  {
    if (context != null)
    {
      User = context.User;
    }
  }

  private static string BuildMessage(Type clientType, string methodName, HttpMethod httpMethod, Uri uri, string? content, IRequestContext? context) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ClientType), clientType.GetNamespaceQualifiedName())
    .AddData(nameof(MethodName), methodName)
    .AddData(nameof(HttpMethod), httpMethod.ToString())
    .AddData(nameof(Uri), uri.ToString())
    .AddData(nameof(Content), content, "<null>")
    .AddData(nameof(User), context?.User, "<null>")
    .Build();
}
