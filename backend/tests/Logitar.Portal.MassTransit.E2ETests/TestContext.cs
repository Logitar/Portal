using Logitar.Portal.MassTransit.Messages;

namespace Logitar.Portal.MassTransit;

internal record TestContext
{
  private readonly Dictionary<string, object> _results = [];
  private T? GetResult<T>(string key) => _results.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetResult(string key, object value) => _results[key] = value;

  public SendMessageCommandResult? SendMessageCommand
  {
    get => GetResult<SendMessageCommandResult>(nameof(SendMessageCommand));
    set => SetResult(nameof(SendMessageCommand), value ?? throw new ArgumentNullException(nameof(SendMessageCommand)));
  }

  public int Completed => _results.Count;
  public int Count { get; } = 1;
  public int Percentage => Completed * 100 / Count;
  public bool HasCompleted => Completed == Count;
}
