using Common.ExternalConsole.Server;

namespace Common.ExternalConsole;

public class ExternalConsole : IDisposable
{
    public ExternalConsole()
    {
    }

    public string Name { get; init; } = string.Empty;

    public ConsolesServer? Server { get; init; }

    public bool? TcpConnectionAvailable => Server?.ContainsClient(Name);

    /// <summary>
    /// Handle two streams yourself, provide actions to handle them.
    /// Each action runs in a new thread.
    /// </summary>
    /// <param name="reader">A StreamReader</param>
    /// <param name="writer">A StreamWriter</param>
    public void HandleMessages(Action<StreamReader> reader, Action<StreamWriter> writer)
    {
        if (Server is null) return;
        var streams = Server?.GetStreams(Name);
        while (streams is null)
            streams = Server?.GetStreams(Name);
        new Thread(() => reader.Invoke(streams.StreamReader)).Start();
        new Thread(() => writer.Invoke(streams.StreamWriter)).Start();
    }

    public void Dispose()
    {
        var available = TcpConnectionAvailable;
        if (available is not null && (bool)available)
            Server?.DisconnectClient(Name);

        GC.SuppressFinalize(this);
    }
}