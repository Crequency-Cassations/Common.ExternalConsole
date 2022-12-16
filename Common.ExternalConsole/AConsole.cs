namespace Common.ExternalConsole;

public class AConsole : IDisposable
{
    private string _name = string.Empty;
    
    public AConsole()
    {
        
    }

    public string Name 
    {
        get => _name;
        set
        {
            _name = value;
        }
    }

    public void Dispose()
    {
        
    }
}