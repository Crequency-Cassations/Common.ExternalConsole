using System.IO.Pipes;

namespace Common.ExternalConsole.Server;

public class AServer : IDisposable
{
    private NamedPipeServerStream? _pipeServer;
    private StreamReader? _streamReader;
    private StreamWriter? _streamWriter;
    private bool _isRunning = true;
    private readonly Queue<string> _receivedMessages = new();
    private readonly Queue<string> _2SendMessages = new();
    private string _name = string.Empty;
    private int _bufferSize = 1024 * 1024;
    private string _ex1 = string.Empty;
    private string _ex2 = string.Empty;

    public AServer()
    {
    }

    /// <summary>
    /// 初始化服务器
    /// </summary>
    /// <param name="name">服务器名称</param>
    /// <param name="bufferSize">缓冲区大小</param>
    /// <returns>服务器实例</returns>
    public AServer Init(string name, int bufferSize = 1024 * 1024)
    {
        _name = name;
        _bufferSize = bufferSize;
        _pipeServer = new($"CommonExternalConsole{name}",
            PipeDirection.InOut, 1,
            PipeTransmissionMode.Byte, PipeOptions.Asynchronous,
            bufferSize, bufferSize);
        _streamReader = new(_pipeServer);
        _streamWriter = new(_pipeServer);
        return this;
    }

    /// <summary>
    /// 开始连接
    /// </summary>
    /// <returns>服务器实例</returns>
    public AServer Start()
    {
        if (!_isRunning) Init(_name, _bufferSize);
        _isRunning = true;

        //  Read Thread
        new Thread(() =>
        {
            try
            {
                _pipeServer?.WaitForConnection();
                while (_isRunning)
                {
                    if (_pipeServer is { CanRead: true })
                    {
                        _receivedMessages.Enqueue(_streamReader?.ReadLine() ?? "null");
                    }
                }
            }
            catch (Exception ex)
            {
                _ex1 = ex.Message;
                _ex2 = _ex1;
            }
        }).Start();

        //  Write Thread
        new Thread(() =>
        {
            try
            {
                while (_isRunning)
                {
                    if (_pipeServer is not { CanWrite: true }) continue;
                    while (_2SendMessages.Count > 0)
                    {
                        _streamWriter?.WriteLine(_2SendMessages.Dequeue());
                        _streamWriter?.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                _ex2 = ex.Message;
                _ex1 = _ex2;
            }
        }).Start();
        
        return this;
    }

    /// <summary>
    /// 写入一行消息
    /// </summary>
    /// <param name="msg">消息</param>
    /// <returns>服务器实例</returns>
    public AServer WriteLine(string msg)
    {
        _2SendMessages.Enqueue(msg);
        return this;
    }

    /// <summary>
    /// 读取一行消息, 若没有可读取的消息则返回 null
    /// </summary>
    /// <returns>消息</returns>
    public string? ReadLine() => _receivedMessages.Count > 0 ? _receivedMessages.Dequeue() : null;

    /// <summary>
    /// 停止服务器进程
    /// </summary>
    /// <returns>服务器实例</returns>
    public AServer Stop()
    {
        _isRunning = false;
        _streamReader?.Close();
        _streamWriter?.Close();
        _pipeServer?.Close();
        return this;
    }

    public void Dispose()
    {
        _streamReader?.Dispose();
        _streamWriter?.Dispose();
        _pipeServer?.Dispose();

        GC.SuppressFinalize(this);
    }
}
