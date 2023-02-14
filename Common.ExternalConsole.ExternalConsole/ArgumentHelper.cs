namespace Common.ExternalConsole.ExternalConsole;

public static class ArgumentHelper
{
    public static ProgramStartupConfig? Process(string[] args)
    {
        var name = "";
        var port = 0;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--port":
                    if (i != args.Length - 1)
                    {
                        ++i;
                        if (int.TryParse(args[i], out var inputPort))
                            port = inputPort;
                        else Console.WriteLine("Invalid user-input port.");
                    }
                    else
                    {
                        Console.WriteLine("No port for connection!");
                        return null;
                    }

                    break;
                case "--name":
                    if (i != args.Length - 1)
                    {
                        ++i;
                        name = args[i];
                    }
                    else
                    {
                        Console.WriteLine("No name for client!");
                        return null;
                    }

                    break;
            }
        }

        return new ProgramStartupConfig(name, port);
    }
}