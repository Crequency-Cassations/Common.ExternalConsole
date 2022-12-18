using System.Diagnostics;
using Common.ExternalConsole;

Console.WriteLine("Hello, Common.ExternalConsole!");

var manager = new Manager();
var console = manager.Register("Console");
console.Start();

var isWaiting = true;

ProcessStartInfo psi = new()
{
    FileName = Path.GetFullPath("./Common.ExternalConsole.Console.exe"),
    Arguments = "--connect CommonExternalConsoleConsole",
    CreateNoWindow = false,
    UseShellExecute = true,
};
Process.Start(psi);

new Thread(() =>
{
    try
    {
        while (true)
        {
            var remote = console.ReadLine();
            if (remote is null) continue;
            Console.WriteLine($"{(isWaiting ? "\r\n" : "")}^ {remote}");
            isWaiting = false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"@ {ex.Message}");
    }
}).Start();

while (true)
{
    Console.Write("$ ");
    isWaiting = true;
    var input = Console.ReadLine();
    isWaiting = false;
    if (input is null) continue;
    if (input == "exit") break;
    console.WriteLine(input);
}

manager.RemoveConsole("Console");