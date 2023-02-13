namespace Common.ExternalConsole.ExternalConsole;

public static class CursorHelper
{
    public static void MoveCursorToLineStart() => Console.SetCursorPosition(0, Console.GetCursorPosition().Top);

    public static void MoveCursorAbove(int distance = 1)
    {
        var position = Console.GetCursorPosition();
        var endPoint = position.Top - distance;
        Console.SetCursorPosition(position.Left, endPoint >= 0 ? endPoint : 0);
    }

    public static void MoveCursorDown(int distance = 1)
    {
        var position = Console.GetCursorPosition();
        var endPoint = position.Top + distance;
        Console.SetCursorPosition(position.Left,
            endPoint <= Console.BufferHeight ? endPoint : Console.BufferHeight
        );
    }

    public static void MoveCursorLeft(int distance = 1)
    {
        var position = Console.GetCursorPosition();
        var endPoint = position.Left - distance;
        Console.SetCursorPosition(endPoint >= 0 ? endPoint : 0, position.Top);
    }

    public static void MoveCursorRight(int distance = 1)
    {
        var position = Console.GetCursorPosition();
        var endPoint = position.Left + distance;
        Console.SetCursorPosition(
            endPoint <= Console.BufferWidth ? endPoint : Console.BufferWidth,
            position.Top
        );
    }
}