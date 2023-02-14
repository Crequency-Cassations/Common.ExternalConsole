using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Common.ExternalConsole.Server;

public class ConsolesServer : IDisposable
{
    private readonly IPAddress _serverIpAddress = IPAddress.Parse("127.0.0.1");
    private TcpListener? _tcpListener;
    private readonly Dictionary<string, TcpClient> _tcpClients = new();
    private readonly Dictionary<string, StreamsRecord> _tcpStreams = new();
    private bool keepListen = true;

    private const string ConsoleTitle = "Common.ExternalConsole.Console | Name: ";

    /// <summary>
    /// Set to false to stop listening.
    /// But you can't set this to true to start listening, please call StartListen(int port)
    /// </summary>
    public bool KeepListen
    {
        get => keepListen;
        set
        {
            if (keepListen && !value)
                StopListening();
            keepListen = value;
        }
    }

    /// <summary>
    /// Get current port of TcpListener, if didn't started listening than return null.
    /// </summary>
    public int? CurrentPort => ((IPEndPoint)_tcpListener?.LocalEndpoint!)?.Port;

    /// <summary>
    /// Start to listen tcp requests and detect weather ExternalConsole instance.
    /// </summary>
    /// <param name="port">Port</param>
    /// <returns>This</returns>
    public async Task<ConsolesServer> StartListen(int port)
    {
        _tcpListener = new TcpListener(_serverIpAddress, port);
        _tcpListener.Start();

        async void Start()
        {
            await StartAcceptingClient();
        }

        await Task.Run(() => new Thread(Start).Start());

        return this;
    }

    /// <summary>
    /// Stop listening.
    /// </summary>
    public void StopListening()
    {
        _tcpListener?.Stop();
        GC.Collect();
    }

    /// <summary>
    /// Start accepting tcp clients, if ExternalConsole instance than add to _tcpClients.
    /// </summary>
    /// <returns>This</returns>
    private async Task<ConsolesServer> StartAcceptingClient()
    {
        if (_tcpListener is null) return this;

        while (keepListen)
        {
            if (_tcpListener.Pending())
                await HandleNewClient(await _tcpListener.AcceptTcpClientAsync());
            else await Task.Delay(500);
        }

        return this;
    }

    /// <summary>
    /// Handle new client, firstly read to end and check if ExternalConsole instance, true than add to _tcpClients.
    /// </summary>
    /// <param name="client">TcpClient</param>
    private async Task HandleNewClient(TcpClient client)
    {
        while (client.Available <= 0)
        {
        }

        var ns = client.GetStream();
        var sr = new StreamReader(ns, Encoding.UTF8);

        async void CancelThis()
        {
            sr.Close();
            sr.Dispose();
            ns.Close();
            await ns.DisposeAsync();
        }

        var msg = await sr.ReadLineAsync();

        if (msg is null)
        {
            CancelThis();
            return;
        }

        if (msg.StartsWith(ConsoleTitle))
        {
            var name = msg[ConsoleTitle.Length..];
            _tcpClients.Add(name, client);

            var sw = new StreamWriter(ns, Encoding.UTF8);
            await sw.WriteLineAsync("Welcome to Common.ExternalConsole!");
            await sw.FlushAsync();

            _tcpStreams.Add(name, new StreamsRecord(sr, sw));
        }
        else CancelThis();
    }

    /// <summary>
    /// Return a bool indicates whether specific client connected.
    /// </summary>
    /// <param name="name">Client name</param>
    /// <returns>Whether specific client connected</returns>
    public bool ContainsClient(string name) => _tcpClients.ContainsKey(name);

    /// <summary>
    /// Get client instance by name.
    /// If client with specific name doesn't exists, return null. 
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>TcpClient instance</returns>
    public TcpClient? GetClient(string name)
    {
        return _tcpClients.TryGetValue(name, out var result) ? result : null;
    }

    /// <summary>
    /// Get two streams about a StreamReader and a StreamWriter of specific client by name. 
    /// </summary>
    /// <param name="name">Client name</param>
    /// <returns>Two streams in a record</returns>
    public StreamsRecord? GetStreams(string name)
    {
        return _tcpStreams.ContainsKey(name) ? _tcpStreams[name] : null;
        return _tcpStreams.TryGetValue(name, out var streams) ? streams : null;
    }

    /// <summary>
    /// Disconnect a client by name.
    /// </summary>
    /// <param name="name">Client name</param>
    /// <returns>If client exists, remove and than return this. Otherwise, return null.</returns>
    public ConsolesServer? DisconnectClient(string name)
    {
        if (!_tcpClients.TryGetValue(name, out var client)) return null;

        client.Close();
        client.Dispose();
        _tcpClients.Remove(name);
        _tcpStreams.Remove(name);
        GC.Collect();
        return this;
    }

    /// <summary>
    /// Dispose this instance.
    /// </summary>
    public void Dispose()
    {
        _tcpListener?.Stop();

        GC.SuppressFinalize(this);
    }
}
