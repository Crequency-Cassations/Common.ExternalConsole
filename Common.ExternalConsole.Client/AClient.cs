using System.IO.Pipes;

namespace Common.ExternalConsole.Client;

public class AClient : IDisposable
{
    private NamedPipeClientStream? _pipeClient;
    private StreamReader? _streamReader;
    private StreamWriter? _streamWriter;
    private bool _isRunning = true;
    private readonly Queue<string> _receivedMessages = new();
    private readonly Queue<string> _2SendMessages = new();
    private string? _pipeName = string.Empty;

    public AClient()
    {
    }

    public AClient Init(string pipeName)
    {
        _pipeName = pipeName;
        _pipeClient = new("localhost", pipeName,
            PipeDirection.InOut, PipeOptions.Asynchronous);
        _streamReader = new(_pipeClient);
        _streamWriter = new(_pipeClient);
        return this;
    }

    public AClient Start()
    {
        if (_pipeName == null) throw new Exception("Didn't Init()");
        if (!_isRunning) Init(_pipeName);
        _isRunning = true;
        _pipeClient?.Connect();

        //  Read Thread
        new Thread(() =>
        {
            while (_isRunning)
            {
                if (_pipeClient is { CanRead: true })
                {
                    _receivedMessages.Enqueue(_streamReader?.ReadLine() ?? "null");
                }
            }
        }).Start();

        //  Write Thread
        new Thread(() =>
        {
            while (_isRunning)
            {
                if (_pipeClient is not { CanWrite: true }) continue;
                while (_2SendMessages.Count > 0)
                {
                    _streamWriter?.WriteLine(_2SendMessages.Dequeue());
                    _streamWriter?.Flush();
                }
            }
        }).Start();

        return this;
    }

    public AClient Stop()
    {
        _isRunning = false;
        _streamReader?.Close();
        _streamWriter?.Close();
        _pipeClient?.Close();
        return this;
    }

    public AClient WriteLine(string msg)
    {
        _2SendMessages.Enqueue(msg);
        return this;
    }

    public string? ReadLine() => _receivedMessages.Count > 0 ? _receivedMessages.Dequeue() : null;

    public void Dispose()
    {
        _streamReader?.Dispose();
        _streamWriter?.Dispose();
        _pipeClient?.Dispose();
    }
}