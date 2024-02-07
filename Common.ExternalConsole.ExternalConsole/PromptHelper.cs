namespace Common.ExternalConsole.ExternalConsole;

public static class PromptHelper
{
    private static string _userPrompt = "$ ";
    private static string _remotePrompt = "# ";
    private static string _debugPrompt = "& ";

    internal static object ConsoleWriteLock = new();

    public static string UserPrompt
    {
        set => _userPrompt = value;
    }

    public static string RemotePrompt
    {
        set => _remotePrompt = value;
    }

    public static string DebugPrompt
    {
        set => _debugPrompt = value;
    }

    public static bool DebugEnabled { get; set; } = false;

    private static void Action(Action action, ConsoleColor cc = ConsoleColor.White)
    {
        var originColor = Console.ForegroundColor;
        Console.ForegroundColor = cc;
        action.Invoke();
        Console.ForegroundColor = originColor;
    }

    public static void User() => Print("$ ", false);

    private static void Normally(string prompt, string content, ConsoleColor cc = ConsoleColor.White,
        Action<(int Left, int Top), string[]>? action = null, bool disableLineStartCheck = false)
    {
        lock (ConsoleWriteLock)
        {
            var position = Console.GetCursorPosition();
            if (!disableLineStartCheck && position.Left != 0)
                Console.WriteLine();

            var lines = content.Split('\n');

            Action(() =>
            {
                if (lines is null) return;
                foreach (var line in lines)
                {
                    Console.Write(prompt);
                    Console.WriteLine(line);
                }
            }, cc);
            action?.Invoke(position, lines);
        }
    }

    public static void Local(string content, ConsoleColor cc = ConsoleColor.White) =>
        Normally(_userPrompt ?? "$ ", content, cc);

    public static void Remote(string content, ConsoleColor cc = ConsoleColor.Cyan)
        => Normally(_remotePrompt ?? "# ", content, cc,
            (position, lines) =>
            {
                var top = position.Top + lines.Length + 1;
                top = top >= Console.BufferHeight ? Console.BufferHeight - 1 : top;
                Console.SetCursorPosition(0, top);
                User();
            });

    public static void Debug(string content, ConsoleColor cc = ConsoleColor.Yellow)
    {
        if (DebugEnabled)
        {
            Normally(_debugPrompt ?? "& ", content, cc,
                (position, lines) =>
                {
                    var top = position.Top + lines.Length + 1;
                    top = top >= Console.BufferHeight ? Console.BufferHeight - 1 : top;
                    Console.SetCursorPosition(0, top);
                    User();
                });
        }
    }

    public static void Print(string content, bool newLine = true)
    {
        lock (ConsoleWriteLock)
        {
            if (newLine) Console.WriteLine(content);
            else Console.Write(content);
        }
    }
}
