namespace Common.ExternalConsole.ExternalConsole;

public static class PromptHelper
{
    private static string _userPrompt = "$ ";
    private static string _remotePrompt = "# ";
    private static string _debugPrompt = "& ";

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

    public static void User() => Console.Write("$ ");

    private static void Normally(string prompt, string content, ConsoleColor cc = ConsoleColor.White,
        Action<(int Left, int Top)>? action = null, bool disableLineStartCheck = false)
    {
        var position = Console.GetCursorPosition();
        if (!disableLineStartCheck && position.Left != 0)
            Console.WriteLine();

        Action(() =>
        {
            Console.Write(prompt);
            Console.WriteLine(content);
        }, cc);
        action?.Invoke(position);
    }

    public static void Local(string content, ConsoleColor cc = ConsoleColor.White) =>
        Normally(_userPrompt ?? "$ ", content, cc);

    public static void Remote(string content, ConsoleColor cc = ConsoleColor.Cyan)
        => Normally(_remotePrompt ?? "# ", content, cc,
            (position) =>
            {
                Console.SetCursorPosition(0, position.Top);
                User();
            });

    public static void Debug(string content, ConsoleColor cc = ConsoleColor.Yellow)
    {
        if (DebugEnabled)
        {
            Normally(_debugPrompt ?? "& ", content, cc,
                (position) =>
                {
                    Console.SetCursorPosition(0, position.Top);
                    User();
                });
        }
    }
}