namespace Common.ExternalConsole;

public class Manager
{
    private readonly object _consolesLock = new();
    private readonly Dictionary<string, AConsole> _consoles = new();

    public Manager()
    {
    }

    /// <summary>
    /// 注册新控制台
    /// 若存在相同名称的控制台则返回该控制台
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>控制台实例</returns>
    public AConsole Register(string name)
    {
        lock (_consolesLock)
        {
            if (_consoles.ContainsKey(name))
                return _consoles[name];
            else
            {
                AConsole console = new()
                {
                    Name = name
                };
                _consoles.Add(name, console);
                return console;
            }
        }
    }

    /// <summary>
    /// 获取控制台实例
    /// </summary>
    /// <param name="name">控制台实例名称</param>
    /// <returns>控制台实例</returns>
    public AConsole? GetConsole(string name)
    {
        lock (_consolesLock)
        {
            return _consoles.ContainsKey(name) ? _consoles[name] : null;
        }
    }

    /// <summary>
    /// 移除控制台实例并销毁
    /// </summary>
    /// <param name="name">控制台实例名称</param>
    public void RemoveConsole(string name)
    {
        lock (_consolesLock)
        {
            if (!_consoles.ContainsKey(name)) return;
            _consoles[name].Dispose();
            _consoles.Remove(name);
        }
    }
}