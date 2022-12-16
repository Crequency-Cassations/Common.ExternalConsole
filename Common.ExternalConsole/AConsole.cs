using Common.ExternalConsole.Server;

namespace Common.ExternalConsole;

public class AConsole : IDisposable
{
    private string _name = string.Empty;
    private readonly AServer _server = new();

    public AConsole()
    {
    }

    public string Name
    {
        get => _name;
        set { _name = value; }
    }

    public AConsole Start(int bufferSize = 1024 * 1024)
    {
        _ = _server.Init(Name, bufferSize);
        _ = _server.Start();
        return this;
    }

    public string? ReadLine() => _server.ReadLine();

    public AConsole WriteLine(string msg)
    {
        _ = _server.WriteLine(msg);
        return this;
    }

    public AConsole Stop()
    {
        _server.Stop();
        return this;
    }

    public void Dispose()
    {
        _server.Dispose();
    }
}