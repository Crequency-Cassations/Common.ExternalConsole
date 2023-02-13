using Common.ExternalConsole.Server;

namespace Common.ExternalConsole;

public class ExternalConsolesManager
{
    private readonly object _consolesCollectionLock = new();
    private readonly Dictionary<string, ExternalConsole> _consoles = new();
    private readonly ConsolesServer _consolesServer = new();

    public ExternalConsolesManager()
    {
            
    }

    /// <summary>
    /// 注册一个控制台管理实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <returns>注册的控制台管理实例</returns>
    public ExternalConsole Register(string name)
    {
        lock (_consolesCollectionLock)
        {
            if (_consoles.ContainsKey(name))
                return _consoles[name];
            else
            {
                var console = new ExternalConsole()
                {
                    Name = name,
                    Server = _consolesServer,
                };
                _consoles.Add(name, console);
                return console;
            }
        }
    }

    /// <summary>
    /// 访问一个已注册的控制台管理实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <returns>若存在, 则返回实例, 否则返回 null</returns>
    public ExternalConsole? GetConsole(string name)
    {
        lock (_consolesCollectionLock)
        {
            return _consoles.TryGetValue(name, out var console) ? console : null;
        }
    }

    /// <summary>
    /// 释放一个控制台管理实例
    /// </summary>
    /// <param name="name">实例名称</param>
    public void DisposeConsole(string name)
    {
        lock (_consolesCollectionLock)
        {
            if (!_consoles.ContainsKey(name)) return;
            _consoles[name].Dispose();
            _consoles.Remove(name);
            GC.Collect();
        }
    }

    /// <summary>
    /// 启动服务器, 默认端口为 0
    /// </summary>
    /// <param name="port">端口</param>
    /// <returns>控制台服务器实例</returns>
    public async Task<ExternalConsolesManager> LaunchServer(int port = 0)
    {
        await _consolesServer.StartListen(port);

        return this;
    }

}