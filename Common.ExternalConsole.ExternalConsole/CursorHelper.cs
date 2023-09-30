namespace Common.ExternalConsole.ExternalConsole;

public static class CursorHelper
{
    public static void MoveCursorToLineStart() => Console.SetCursorPosition(0, Console.GetCursorPosition().Top);

    public static void MoveCursorAbove(int distance = 1)
    {
        var (Left, Top) = Console.GetCursorPosition();
        var endPoint = Top - distance;
        Console.SetCursorPosition(Left, endPoint >= 0 ? endPoint : 0);
    }

    public static void MoveCursorDown(int distance = 1)
    {
        var (Left, Top) = Console.GetCursorPosition();
        var endPoint = Top + distance;
        Console.SetCursorPosition(Left,
            endPoint <= Console.BufferHeight ? endPoint : Console.BufferHeight
        );
    }

    public static void MoveCursorLeft(int distance = 1)
    {
        var (Left, Top) = Console.GetCursorPosition();
        var endPoint = Left - distance;
        Console.SetCursorPosition(endPoint >= 0 ? endPoint : 0, Top);
    }

    public static void MoveCursorRight(int distance = 1)
    {
        var (Left, Top) = Console.GetCursorPosition();
        var endPoint = Left + distance;
        Console.SetCursorPosition(
            endPoint <= Console.BufferWidth ? endPoint : Console.BufferWidth,
            Top
        );
    }
}
