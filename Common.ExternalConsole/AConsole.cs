using Common.ExternalConsole.Server;

namespace Common.ExternalConsole;

public class AConsole : IDisposable
{
    private string _name = string.Empty;
    private AServer _server = new();

    public AConsole()
    {
    }

    public string Name
    {
        get => _name;
        set { _name = value; }
    }

    public void Dispose()
    {
    }
}